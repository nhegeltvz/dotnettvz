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
                errors.Add(new Error(ErrorType.Validation, "Scheduled match is required."));
            }
            else if (!_dbContext.ScheduledMatches.Any(match => match.Id == entity.ScheduledMatchId))
            {
                errors.Add(new Error(ErrorType.Validation, "Scheduled match does not exist."));
            }

            if (entity.PlayerId == Guid.Empty)
            {
                errors.Add(new Error(ErrorType.Validation, "Player is required."));
            }
            else if (!_dbContext.Users.Any(player => player.Id == entity.PlayerId))
            {
                errors.Add(new Error(ErrorType.Validation, "Player does not exist."));
            }

            if (entity.ScheduledMatchId != Guid.Empty && entity.PlayerId != Guid.Empty)
            {
                var duplicate = _dbContext.ScheduledMatchAttendances.Any(attendance =>
                    attendance.Id != entity.Id
                    && attendance.ScheduledMatchId == entity.ScheduledMatchId
                    && attendance.PlayerId == entity.PlayerId);
                if (duplicate)
                {
                    errors.Add(new Error(ErrorType.Validation, "Attendance already exists for this player."));
                }
            }

            return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
        }
    }
}
