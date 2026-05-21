using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            if (string.IsNullOrWhiteSpace(entity.Username))
            {
                errors.Add(new Error(ErrorType.Validation, "Username is required."));
            }
            else if (entity.Username.Trim().Length < 3)
            {
                errors.Add(new Error(ErrorType.Validation, "Username must be at least 3 characters."));
            }

            if (string.IsNullOrWhiteSpace(entity.Email))
            {
                errors.Add(new Error(ErrorType.Validation, "Email is required."));
            }
            else
            {
                var email = entity.Email.Trim();
                var atIndex = email.IndexOf('@');
                var dotIndex = email.LastIndexOf('.');
                if (atIndex <= 0 || dotIndex <= atIndex + 1 || dotIndex == email.Length - 1)
                {
                    errors.Add(new Error(ErrorType.Validation, "Email format is invalid."));
                }
            }

            if (entity.Age.HasValue && (entity.Age.Value < 18 || entity.Age.Value > 100))
            {
                errors.Add(new Error(ErrorType.Validation, "Age must be between 18 and 100."));
            }

            if (!string.IsNullOrWhiteSpace(entity.Username))
            {
                var username = entity.Username.Trim();
                var usernameExists = _dbContext.Players.Any(player =>
                    player.Id != entity.Id && EF.Functions.Like(player.Username, username));
                if (usernameExists)
                {
                    errors.Add(new Error(ErrorType.Validation, "Username is already in use."));
                }
            }

            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                var email = entity.Email.Trim();
                var emailExists = _dbContext.Players.Any(player =>
                    player.Id != entity.Id && EF.Functions.Like(player.Email, email));
                if (emailExists)
                {
                    errors.Add(new Error(ErrorType.Validation, "Email is already in use."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
