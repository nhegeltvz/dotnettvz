using Data.Data;
using Data.Dto.CRUD.Player;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Dashboard;

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
            .Where(p => string.IsNullOrEmpty(search) || EF.Functions.Like(p.User.UserName, $"%{search}%"))
            .Select(PlayerDto.ToDto())
            .ToListAsync();
        return Json(players);
    }

    [HttpGet("form")]
    public async Task<IActionResult> Form(Guid? id)
    {
        // Users that already have a player profile
        var takenUserIds = await _store.QueryPlayersAsync()
            .Select(p => p.UserId)
            .ToListAsync();

        // If editing, the current player's UserId should remain available
        if (id.HasValue)
        {
            var existing = await _store.FindByIdAsync(id.Value);
            if (existing.IsSuccess)
                takenUserIds.Remove(existing.Value!.UserId);
        }

        var availableUsers = await _userManager.Users
            .Where(u => !takenUserIds.Contains(u.Id))
            .OrderBy(u => u.UserName)
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName ?? u.Email ?? u.Id.ToString(),
            })
            .ToListAsync();

        var vm = new PlayerFormViewModel { Users = availableUsers };

        if (id.HasValue)
        {
            var playerResult = await _store.FindByIdAsync(id.Value);
            if (!playerResult.IsSuccess)
                return NotFound();

            var player = playerResult.Value!;
            vm.Form.Id = player.Id;
            vm.Form.UserId = player.UserId;
            vm.Form.Bio = player.Bio;
            vm.Form.PreferredPosition = (int)player.PreferredPosition;
            vm.Form.DateOfBirth = player.DateOfBirth;
        }

        return PartialView("_PlayerForm", vm);
    }

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _store.QueryPlayersAsync()
            .Where(p => p.Id == id)
            .Select(PlayerDto.ToDto())
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Json(dto);
    }

    // Creates a Player profile for an existing AppUser (admin selects user from dropdown)
    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] PlayerFormDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.UserId == Guid.Empty)
            return BadRequest("Please select a user.");

        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            return BadRequest("User not found.");

        var existingPlayer = await _store.FindByUserIdAsync(model.UserId);
        if (existingPlayer.IsSuccess)
            return BadRequest("This user already has a player profile.");

        var player = new Player
        {
            Id = Guid.NewGuid(),
            UserId = model.UserId,
            Bio = model.Bio,
            PreferredPosition = (Position)model.PreferredPosition,
            DateOfBirth = model.DateOfBirth,
        };

        var result = await _store.CreatePlayer(player);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }

    // Updates Player domain fields only
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
        player.UserId = model.UserId;
        player.Bio = model.Bio;
        player.PreferredPosition = (Position)model.PreferredPosition;
        player.DateOfBirth = model.DateOfBirth;

        var updateResult = await _store.UpdatePlayer(player);
        if (!updateResult.IsSuccess)
            return BadRequest(updateResult.Errors);

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
