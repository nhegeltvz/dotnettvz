using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class PartyValidator : IValidator<Party>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PartyValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(Party entity)
        {
            var errors = new List<Error>();

            if (entity.PlayerCreatedId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Party owner is required."));
            }
            else if (!_dbContext.Players.Any(p => p.Id == entity.PlayerCreatedId))
            {
                errors.Add(new Error(ErrorType.Validation, "Party owner does not exist."));
            }

            if (entity.DateCreated > DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Date created cannot be in the future."));
            }

            if (entity.MaxMembers <= 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Max members must be greater than zero."));
            }

            if (string.IsNullOrWhiteSpace(entity.PartyDescription))
            {
                errors.Add(new Error(ErrorType.Validation, "Party description is required."));
            }

            if (string.IsNullOrWhiteSpace(entity.PreferredLocations))
            {
                errors.Add(new Error(ErrorType.Validation, "Preferred locations are required."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
