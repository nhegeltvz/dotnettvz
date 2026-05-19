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
                errors.Add(new Error(ErrorType.Validation, "Playing field is required."));
            }
            else if (!_dbContext.PlayingFields.Any(field => field.Id == entity.PlayingFieldId))
            {
                errors.Add(new Error(ErrorType.Validation, "Playing field does not exist."));
            }

            if (entity.GoalsTeamA < 0 || entity.GoalsTeamB < 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Goals cannot be negative."));
            }

            if (!entity.WasMatchHeld && (entity.GoalsTeamA > 0 || entity.GoalsTeamB > 0))
            {
                errors.Add(new Error(ErrorType.Validation, "Goals must be zero if the match was not held."));
            }

            if (entity.WasMatchHeld && entity.MatchHeld > DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Match date cannot be in the future."));
            }

            if (entity.MatchHeld == default)
            {
                errors.Add(new Error(ErrorType.Validation, "Match date is required."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
