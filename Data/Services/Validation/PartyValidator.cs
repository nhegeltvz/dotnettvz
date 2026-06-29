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
                errors.Add(new Error(ErrorType.Validation, "Vlasnik grupe je obavezan."));
            }
            else if (!_dbContext.Players.Any(p => p.Id == entity.PlayerCreatedId))
            {
                errors.Add(new Error(ErrorType.Validation, "Vlasnik grupe ne postoji."));
            }

            if (entity.DateCreated > DateTime.UtcNow)
            {
                errors.Add(new Error(ErrorType.Validation, "Datum kreiranja ne može biti u budućnosti."));
            }

            if (entity.MaxMembers <= 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Maksimalan broj članova mora biti veći od nule."));
            }

            if (string.IsNullOrWhiteSpace(entity.PartyDescription))
            {
                errors.Add(new Error(ErrorType.Validation, "Opis grupe je obavezan."));
            }

            if (string.IsNullOrWhiteSpace(entity.PreferredLocations))
            {
                errors.Add(new Error(ErrorType.Validation, "Preferirane lokacije su obavezne."));
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
