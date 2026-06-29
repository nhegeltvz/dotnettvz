using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PlayerValidator : IValidator<Player>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PlayerValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(Player entity)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(entity.Bio))
            {
                errors.Add(new Error(ErrorType.Validation, "Biografija je obavezna."));
            }
            else if (entity.Bio.Trim().Length < 10)
            {
                errors.Add(new Error(ErrorType.Validation, "Biografija mora imati najmanje 10 znakova."));
            }

            if (entity.DateOfBirth == default)
            {
                errors.Add(new Error(ErrorType.Validation, "Datum rođenja je obavezan."));
            }
            else
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var age = today.Year - entity.DateOfBirth.Year;
                if (entity.DateOfBirth > today.AddYears(-age)) age--;

                if (age < 18 || age > 100)
                    errors.Add(new Error(ErrorType.Validation, "Igrač mora imati između 18 i 100 godina."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
