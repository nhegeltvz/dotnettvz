using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PlayingFieldValidator : IValidator<PlayingField>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PlayingFieldValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(PlayingField entity)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                errors.Add(new Error(ErrorType.Validation, "Name is required."));
            }
            else
            {
                var name = entity.Name.Trim().ToLowerInvariant();
                var nameExists = _dbContext.PlayingFields.Any(field =>
                    field.Id != entity.Id && field.Name.ToLowerInvariant() == name);
                if (nameExists)
                {
                    errors.Add(new Error(ErrorType.Validation, "Playing field name is already in use."));
                }
            }

            if (entity.Latitude < -90 || entity.Latitude > 90)
            {
                errors.Add(new Error(ErrorType.Validation, "Latitude must be between -90 and 90."));
            }

            if (entity.Longitude < -180 || entity.Longitude > 180)
            {
                errors.Add(new Error(ErrorType.Validation, "Longitude must be between -180 and 180."));
            }

            if (string.IsNullOrWhiteSpace(entity.ContactNumber))
            {
                errors.Add(new Error(ErrorType.Validation, "Contact number is required."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
