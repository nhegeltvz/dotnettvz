using Data.Dto.CRUD.PlayerRating;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("player-ratings")]
public class PlayerRatingsController : Controller
{
    private readonly PlayerRatingStore _store;

    public PlayerRatingsController(PlayerRatingStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var ratings = await _store.GetAllPlayerRatingsAsync();
        return PartialView("~/Views/Dashboard/PlayerRatings/_List.cshtml", ratings);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", new PlayerRatingFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(PlayerRatingFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", model);
        }

        var rating = new PlayerRating
        {
            Id = Guid.NewGuid(),
            MatchPlayerId = model.MatchPlayerId,
            PlayerGivingRatingId = model.PlayerGivingRatingId,
            PlayerReceivingRatingId = model.PlayerReceivingRatingId,
            Rating = model.Rating,
        };

        var result = await _store.CreatePlayerRating(rating);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var ratingResult = await _store.FindByIdAsync(id);
        if (!ratingResult.IsSuccess)
            return NotFound();

        var rating = ratingResult.Value;
        var model = new PlayerRatingFormDto
        {
            Id = rating.Id,
            MatchPlayerId = rating.MatchPlayerId,
            PlayerGivingRatingId = rating.PlayerGivingRatingId,
            PlayerReceivingRatingId = rating.PlayerReceivingRatingId,
            Rating = rating.Rating,
        };

        return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, PlayerRatingFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", model);
        }

        var ratingResult = await _store.FindByIdAsync(id);
        if (!ratingResult.IsSuccess)
            return NotFound();

        var rating = ratingResult.Value;
        rating.MatchPlayerId = model.MatchPlayerId;
        rating.PlayerGivingRatingId = model.PlayerGivingRatingId;
        rating.PlayerReceivingRatingId = model.PlayerReceivingRatingId;
        rating.Rating = model.Rating;

        var result = await _store.UpdatePlayerRating(rating);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/PlayerRatings/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ratingResult = await _store.FindByIdAsync(id);
        if (!ratingResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete player rating while related data exists.");
        }

        return Ok();
    }
}
