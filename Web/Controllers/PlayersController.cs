using Data.Data;
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

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var players = await _store.GetAllPlayersAsync();
        return View("Players", players);
    }


    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var players = string.IsNullOrWhiteSpace(search)
    ? await _store.GetAllPlayersAsync()
    : await _store.SearchByUsernameAsync(search);
        return Json(players);
    }

    [HttpGet("form")]
    public IActionResult Form() => PartialView("_PlayerForm", new PlayerFormDto());

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        return Json(playerResult.Value);
    }



    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] PlayerFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            Bio = model.Bio,
            PreferredPosition = (Position)model.PreferredPosition,
            Age = model.Age,
        };

        var result = await _store.CreatePlayer(player);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }


    [HttpPut("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] PlayerFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        var player = playerResult.Value;
        player.Username = model.Username;
        player.Email = model.Email;
        player.Bio = model.Bio;
        player.PreferredPosition = (Position)model.PreferredPosition;
        player.Age = model.Age;

        var result = await _store.UpdatePlayer(player);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var playerResult = await _store.FindByIdAsync(id);
        if (!playerResult.IsSuccess)
            return NotFound();

        await _store.DeleteByIdAsync(id);
        return Ok();
    }


}
