using Data.Data;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly PlayerStore _store;
    private readonly UserManager<AppUser> _userManager;

    public PlayersController(PlayerStore store, UserManager<AppUser> userManager)
    {
        _store = store;
        _userManager = userManager;
    }

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
        var players = await _store.QueryPlayersAsync()
            .Where(p => string.IsNullOrEmpty(search) || EF.Functions.Like(p.Username, $"%{search}%"))
            .Select(PlayerDto.ToDto())
            .ToListAsync();
        return Json(players);
    }

    [HttpGet("form")]
    public IActionResult Form() => PartialView("_PlayerForm", new PlayerFormDto());

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _store.QueryPlayersAsync()
            .Where(p => p.Id == id)
            .Select(PlayerDto.ToDto())
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Json(dto);
    }

    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] PlayerFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(model.Password))
            return BadRequest("Password is required when creating a player.");

        var player = new AppUser
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            UserName = model.Email,
            Bio = model.Bio,
            PreferredPosition = (Position)model.PreferredPosition,
            Age = model.Age,
            OIB = model.OIB,
            JMBG = model.JMBG,
        };

        var result = await _userManager.CreateAsync(player, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(player, "User");

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
        player.UserName = model.Email;
        player.Bio = model.Bio;
        player.PreferredPosition = (Position)model.PreferredPosition;
        player.Age = model.Age;
        player.OIB = model.OIB;
        player.JMBG = model.JMBG;

        var updateResult = await _userManager.UpdateAsync(player);
        if (!updateResult.Succeeded)
            return BadRequest(updateResult.Errors.Select(e => e.Description));

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(player);
            var pwResult = await _userManager.ResetPasswordAsync(player, token, model.Password);
            if (!pwResult.Succeeded)
                return BadRequest(pwResult.Errors.Select(e => e.Description));
        }

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
