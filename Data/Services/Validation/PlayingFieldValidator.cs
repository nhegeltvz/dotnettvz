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
                errors.Add(new Error(ErrorType.Validation, "Naziv je obavezan."));
            }
            else
            {
                var name = entity.Name.Trim().ToLowerInvariant();
                var nameExists = _dbContext.PlayingFields.Any(field =>
                    field.Id != entity.Id && field.Name.ToLower() == name);
                if (nameExists)
                {
                    errors.Add(new Error(ErrorType.Validation, "Naziv terena je već u upotrebi."));
                }
            }

            if (entity.Latitude < -90 || entity.Latitude > 90)
            {
                errors.Add(new Error(ErrorType.Validation, "Geografska širina mora biti između -90 i 90."));
            }

            if (entity.Longitude < -180 || entity.Longitude > 180)
            {
                errors.Add(new Error(ErrorType.Validation, "Geografska dužina mora biti između -180 i 180."));
            }

            if (string.IsNullOrWhiteSpace(entity.ContactNumber))
            {
                errors.Add(new Error(ErrorType.Validation, "Kontakt broj je obavezan."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
