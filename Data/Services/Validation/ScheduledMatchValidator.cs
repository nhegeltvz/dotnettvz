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
                errors.Add(new Error(ErrorType.Validation, "Teren je obavezan."));
            }
            else if (!_dbContext.PlayingFields.Any(field => field.Id == entity.PlayingFieldId))
            {
                errors.Add(new Error(ErrorType.Validation, "Teren ne postoji."));
            }

            if (entity.PartyId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Grupa je obavezna."));
            }
            else if (!_dbContext.Parties.Any(party => party.Id == entity.PartyId))
            {
                errors.Add(new Error(ErrorType.Validation, "Grupa ne postoji."));
            }

            if (entity.MatchDate < DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Datum meča mora biti u budućnosti."));
            }

            if (entity.PartyId != Guid.Empty)
            {
                var partyAlreadyScheduled = _dbContext.ScheduledMatches.Any(match =>
                    match.PartyId == entity.PartyId && match.Id != entity.Id);
                if (partyAlreadyScheduled)
                {
                    errors.Add(new Error(ErrorType.Validation, "Grupa već ima zakazan meč."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
