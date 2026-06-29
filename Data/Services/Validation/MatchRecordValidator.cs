using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class MatchRecordValidator : IValidator<MatchRecord>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public MatchRecordValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(MatchRecord entity)
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

            if (entity.GoalsTeamA < 0 || entity.GoalsTeamB < 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Golovi ne mogu biti negativni."));
            }

            if (!entity.WasMatchHeld && (entity.GoalsTeamA > 0 || entity.GoalsTeamB > 0))
            {
                errors.Add(new Error(ErrorType.Validation, "Golovi moraju biti nula ako meč nije odigran."));
            }

            if (entity.WasMatchHeld && entity.MatchHeld > DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Datum meča ne može biti u budućnosti."));
            }

            if (entity.MatchHeld == default)
            {
                errors.Add(new Error(ErrorType.Validation, "Datum meča je obavezan."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
