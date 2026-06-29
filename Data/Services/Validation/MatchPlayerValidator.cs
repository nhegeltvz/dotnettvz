using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class MatchPlayerValidator : IValidator<MatchPlayer>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public MatchPlayerValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(MatchPlayer entity)
        {
            var errors = new List<Error>();

            if (entity.PlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač je obavezan."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač ne postoji."));
            }

            if (entity.MatchRecordId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Utakmica je obavezna."));
            }
            else if (!_dbContext.MatchRecords.Any(record => record.Id == entity.MatchRecordId))
            {
                errors.Add(new Error(ErrorType.Validation, "Utakmica ne postoji."));
            }

            if (!Enum.IsDefined(typeof(Team), entity.Team))
            {
                errors.Add(new Error(ErrorType.Validation, "Odabir ekipe nije ispravan."));
            }

            if (entity.Goals < 0 || entity.Assists < 0)
            {
                errors.Add(new Error(ErrorType.Validation, "Golovi i asistencije ne mogu biti negativni."));
            }

            if (entity.PlayerId != Guid.Empty && entity.MatchRecordId != Guid.Empty)
            {
                var duplicate = _dbContext.MatchPlayers.Any(matchPlayer =>
                    matchPlayer.Id != entity.Id
                    && matchPlayer.PlayerId == entity.PlayerId
                    && matchPlayer.MatchRecordId == entity.MatchRecordId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Igrač je već dodijeljen ovoj utakmici."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
