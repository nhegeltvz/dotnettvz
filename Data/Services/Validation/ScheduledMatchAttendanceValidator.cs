using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Services.Validation.Interfaces;

namespace Data.Services.Validation
{
    public class ScheduledMatchAttendanceValidator : IValidator<ScheduledMatchAttendance>
    {
        private readonly MatchTrackerDbContext _dbContext;

        public ScheduledMatchAttendanceValidator(MatchTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Result Validate(ScheduledMatchAttendance entity)
        {
            var errors = new List<Error>();

            if (entity.ScheduledMatchId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Zakazani meč je obavezan."));
            }
            else if (!_dbContext.ScheduledMatches.Any(match => match.Id == entity.ScheduledMatchId))
            {
                errors.Add(new Error(ErrorType.Validation, "Zakazani meč ne postoji."));
            }

            if (entity.PlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač je obavezan."));
            }
            else if (!_dbContext.Players.Any(player => player.Id == entity.PlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Igrač ne postoji."));
            }

            if (entity.ScheduledMatchId != Guid.Empty && entity.PlayerId != Guid.Empty)
            {
                var duplicate = _dbContext.ScheduledMatchAttendances.Any(attendance =>
                    attendance.Id != entity.Id
                    && attendance.ScheduledMatchId == entity.ScheduledMatchId
                    && attendance.PlayerId == entity.PlayerId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Dolazak za ovog igrača već postoji."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
