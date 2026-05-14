using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class MatchVoteStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<MatchVote> _matchVoteValidator;

        public MatchVoteStore(MatchTrackerDbContext dbContext, IValidator<MatchVote> matchVoteValidator)
        {
            _dbContext = dbContext;
            _matchVoteValidator = matchVoteValidator;
        }

        public async Task<List<MatchVote>> GetAllMatchVotesAsync()
            => await _dbContext.MatchVotes
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<MatchVote>> FindByIdAsync(Guid id)
        {
            var matchVote = await _dbContext.MatchVotes.FindAsync(id);
            return matchVote != null ? Result<MatchVote>.Success(matchVote) : Result<MatchVote>.Failure(MatchVoteErrors.MatchVoteNotFound);
        }

        public async Task<Result<Guid>> CreateMatchVote(IMatchVote model)
        {
            var matchVote = new MatchVote();
            UpdateMatchVote(model, matchVote);
            var validationResult = _matchVoteValidator.Validate(matchVote);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.MatchVotes.Add(matchVote);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(matchVote.Id);
        }

        public async Task<Result> UpdateMatchVote(IMatchVote model)
        {
            var foundMatchVoteResult = await FindByIdAsync(model.Id);

            if (!foundMatchVoteResult.IsSuccess || foundMatchVoteResult.Value == null)
                return foundMatchVoteResult;

            var matchVote = foundMatchVoteResult.Value;
            UpdateMatchVote(model, matchVote);
            var validationResult = _matchVoteValidator.Validate(matchVote);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(MatchVoteErrors.MatchVoteNotUpdated);
        }

        private MatchVote UpdateMatchVote(IMatchVote model, MatchVote matchVote)
        {
            matchVote.MatchRecordId = model.MatchRecordId;
            matchVote.PlayerId = model.PlayerId;
            matchVote.VotedHeld = model.VotedHeld;
            return matchVote;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.MatchVotes
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(MatchVoteErrors.MatchVoteNotDeleted) : Result.Success();
        }
    }
}
