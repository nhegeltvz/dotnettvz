using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class ScheduledMatchValidator : IValidator<ScheduledMatch>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public ScheduledMatchValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(ScheduledMatch entity)
        {
            var errors = new List<Error>();

            if (entity.PlayingFieldId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Playing field is required."));
            }
            else if (!_dbContext.PlayingFields.Any(field => field.Id == entity.PlayingFieldId))
            {
                errors.Add(new Error(ErrorType.Validation, "Playing field does not exist."));
            }

            if (entity.PartyId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Party is required."));
            }
            else if (!_dbContext.Parties.Any(party => party.Id == entity.PartyId))
            {
                errors.Add(new Error(ErrorType.Validation, "Party does not exist."));
            }

            if (entity.MatchDate < DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Match date must be in the future."));
            }

            if (entity.PartyId != Guid.Empty)
            {
                var partyAlreadyScheduled = _dbContext.ScheduledMatches.Any(match =>
                    match.PartyId == entity.PartyId && match.Id != entity.Id);
                if (partyAlreadyScheduled)
                {
                    errors.Add(new Error(ErrorType.Validation, "Party already has a scheduled match."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
