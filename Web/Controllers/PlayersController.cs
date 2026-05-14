using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly PlayerStore _store;

    public PlayersController(PlayerStore store) => _store = store;

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        return View("PlayerDetailsView", playerResult.Value);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(string? search)
    {
        var players = string.IsNullOrWhiteSpace(search)
            ? await _store.GetAllPlayersAsync()
            : await _store.SearchByUsernameAsync(search);

        return PartialView("~/Views/Dashboard/Players/_List.cshtml", players);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/Players/_Form.cshtml", new PlayerFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PlayerFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Players/_Form.cshtml", model);
        }

        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            Bio = model.Bio,
            PreferredPosition = model.PreferredPosition,
            Age = model.Age,
        };

        var result = await _store.CreatePlayer(player);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Players/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        var player = playerResult.Value;
        var model = new PlayerFormDto
        {
            Id = player.Id,
            Username = player.Username,
            Email = player.Email,
            Bio = player.Bio,
            PreferredPosition = player.PreferredPosition,
            Age = player.Age,
        };

        return PartialView("~/Views/Dashboard/Players/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, PlayerFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Players/_Form.cshtml", model);
        }

        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        var player = playerResult.Value;
        player.Username = model.Username;
        player.Email = model.Email;
        player.Bio = model.Bio;
        player.PreferredPosition = model.PreferredPosition;
        player.Age = model.Age;

        var result = await _store.UpdatePlayer(player);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/Players/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete player while related data exists.");
        }

        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Ok(Array.Empty<string>());

        var names = await _store.SearchUsernamesAsync(term);
        return Ok(names);
    }
}
