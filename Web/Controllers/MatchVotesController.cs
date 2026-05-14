using Data.Dto.CRUD.MatchVote;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[Route("match-votes")]
public class MatchVotesController : Controller
{
    private readonly MatchVoteStore _store;

    public MatchVotesController(MatchVoteStore store) => _store = store;

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var votes = await _store.GetAllMatchVotesAsync();
        return PartialView("~/Views/Dashboard/MatchVotes/_List.cshtml", votes);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", new MatchVoteFormDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(MatchVoteFormDto model)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", model);
        }

        var vote = new MatchVote
        {
            Id = Guid.NewGuid(),
            MatchRecordId = model.MatchRecordId,
            PlayerId = model.PlayerId,
            VotedHeld = model.VotedHeld,
        };

        var result = await _store.CreateMatchVote(vote);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var voteResult = await _store.FindByIdAsync(id);
        if (!voteResult.IsSuccess)
            return NotFound();

        var vote = voteResult.Value;
        var model = new MatchVoteFormDto
        {
            Id = vote.Id,
            MatchRecordId = vote.MatchRecordId,
            PlayerId = vote.PlayerId,
            VotedHeld = vote.VotedHeld,
        };

        return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", model);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, MatchVoteFormDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", model);
        }

        var voteResult = await _store.FindByIdAsync(id);
        if (!voteResult.IsSuccess)
            return NotFound();

        var vote = voteResult.Value;
        vote.MatchRecordId = model.MatchRecordId;
        vote.PlayerId = model.PlayerId;
        vote.VotedHeld = model.VotedHeld;

        var result = await _store.UpdateMatchVote(vote);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Errors!.First().Description);
            Response.StatusCode = 400;
            return PartialView("~/Views/Dashboard/MatchVotes/_Form.cshtml", model);
        }

        return Ok();
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var voteResult = await _store.FindByIdAsync(id);
        if (!voteResult.IsSuccess)
            return NotFound();

        try
        {
            await _store.DeleteByIdAsync(id);
        }
        catch (DbUpdateException)
        {
            return BadRequest("Cannot delete match vote while related data exists.");
        }

        return Ok();
    }
}
