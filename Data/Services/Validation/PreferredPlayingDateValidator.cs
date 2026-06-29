using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PreferredPlayingDateValidator : IValidator<PreferredPlayingDate>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PreferredPlayingDateValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(PreferredPlayingDate entity)
        {
            var errors = new List<Error>();

            if (entity.PartyId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Grupa je obavezna."));
            }
            else if (!_dbContext.Parties.Any(party => party.Id == entity.PartyId))
            {
                errors.Add(new Error(ErrorType.Validation, "Grupa ne postoji."));
            }

            if (entity.Date.Date < DateTime.UtcNow.Date)
            {
                errors.Add(new Error(ErrorType.Validation, "Preferirani datum ne može biti u prošlosti."));
            }

            if (entity.PartyId != Guid.Empty)
            {
                var duplicate = _dbContext.PreferredPlayingDates.Any(preferredDate =>
                    preferredDate.Id != entity.Id
                    && preferredDate.PartyId == entity.PartyId
                    && preferredDate.Date.Date == entity.Date.Date);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Preferirani datum već postoji za ovu grupu."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
