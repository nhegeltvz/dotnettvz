using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class ScheduledMatchStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<ScheduledMatch> _scheduledMatchValidator;

        public ScheduledMatchStore(MatchTrackerDbContext dbContext, IValidator<ScheduledMatch> scheduledMatchValidator)
        {
            _dbContext = dbContext;
            _scheduledMatchValidator = scheduledMatchValidator;
        }

        public async Task<List<ScheduledMatch>> GetAllScheduledMatchesAsync()
            => await _dbContext.ScheduledMatches
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<ScheduledMatch>> FindByIdAsync(Guid id)
        {
            var scheduledMatch = await _dbContext.ScheduledMatches.FindAsync(id);
            return scheduledMatch != null ? Result<ScheduledMatch>.Success(scheduledMatch) : Result<ScheduledMatch>.Failure(ScheduledMatchErrors.ScheduledMatchNotFound);
        }

        public async Task<Result<Guid>> CreateScheduledMatch(IScheduledMatch model)
        {
            var scheduledMatch = new ScheduledMatch();
            UpdateScheduledMatch(model, scheduledMatch);
            var validationResult = _scheduledMatchValidator.Validate(scheduledMatch);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.ScheduledMatches.Add(scheduledMatch);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(scheduledMatch.Id);
        }

        public async Task<Result> UpdateScheduledMatch(IScheduledMatch model)
        {
            var foundScheduledMatchResult = await FindByIdAsync(model.Id);

            if (!foundScheduledMatchResult.IsSuccess || foundScheduledMatchResult.Value == null)
                return foundScheduledMatchResult;

            var scheduledMatch = foundScheduledMatchResult.Value;
            UpdateScheduledMatch(model, scheduledMatch);
            var validationResult = _scheduledMatchValidator.Validate(scheduledMatch);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(ScheduledMatchErrors.ScheduledMatchNotUpdated);
        }

        private ScheduledMatch UpdateScheduledMatch(IScheduledMatch model, ScheduledMatch scheduledMatch)
        {
            scheduledMatch.PlayingFieldId = model.PlayingFieldId;
            scheduledMatch.PartyId = model.PartyId;
            scheduledMatch.MatchDate = model.MatchDate;
            return scheduledMatch;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.ScheduledMatches
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(ScheduledMatchErrors.ScheduledMatchNotDeleted) : Result.Success();
        }
    }
}
