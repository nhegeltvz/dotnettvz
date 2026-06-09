using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class ScheduledMatchAttendanceStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<ScheduledMatchAttendance> _scheduledMatchAttendanceValidator;

        public ScheduledMatchAttendanceStore(MatchTrackerDbContext dbContext, IValidator<ScheduledMatchAttendance> scheduledMatchAttendanceValidator)
        {
            _dbContext = dbContext;
            _scheduledMatchAttendanceValidator = scheduledMatchAttendanceValidator;
        }

        public async Task<List<ScheduledMatchAttendance>> GetAllScheduledMatchAttendancesAsync()
            => await _dbContext.ScheduledMatchAttendances
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<ScheduledMatchAttendance>> GetByScheduledMatchIdAsync(Guid scheduledMatchId)
            => await _dbContext.ScheduledMatchAttendances
                .Where(attendance => attendance.ScheduledMatchId == scheduledMatchId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<ScheduledMatchAttendance>> FindByIdAsync(Guid id)
        {
            var scheduledMatchAttendance = await _dbContext.ScheduledMatchAttendances.FindAsync(id);
            return scheduledMatchAttendance != null ? Result<ScheduledMatchAttendance>.Success(scheduledMatchAttendance) : Result<ScheduledMatchAttendance>.Failure(ScheduledMatchAttendanceErrors.ScheduledMatchAttendanceNotFound);
        }

        public async Task<Result<Guid>> CreateScheduledMatchAttendance(IScheduledMatchAttendance model)
        {
            var scheduledMatchAttendance = new ScheduledMatchAttendance();
            UpdateScheduledMatchAttendance(model, scheduledMatchAttendance);
            var validationResult = _scheduledMatchAttendanceValidator.Validate(scheduledMatchAttendance);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.ScheduledMatchAttendances.Add(scheduledMatchAttendance);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(scheduledMatchAttendance.Id);
        }

        public async Task<Result> UpdateScheduledMatchAttendance(IScheduledMatchAttendance model)
        {
            var foundScheduledMatchAttendanceResult = await FindByIdAsync(model.Id);

            if (!foundScheduledMatchAttendanceResult.IsSuccess || foundScheduledMatchAttendanceResult.Value == null)
                return foundScheduledMatchAttendanceResult;

            var scheduledMatchAttendance = foundScheduledMatchAttendanceResult.Value;
            UpdateScheduledMatchAttendance(model, scheduledMatchAttendance);
            var validationResult = _scheduledMatchAttendanceValidator.Validate(scheduledMatchAttendance);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(ScheduledMatchAttendanceErrors.ScheduledMatchAttendanceNotUpdated);
        }

        private ScheduledMatchAttendance UpdateScheduledMatchAttendance(IScheduledMatchAttendance model, ScheduledMatchAttendance scheduledMatchAttendance)
        {
            scheduledMatchAttendance.ScheduledMatchId = model.ScheduledMatchId;
            scheduledMatchAttendance.PlayerId = model.PlayerId;
            scheduledMatchAttendance.IsAttending = model.IsAttending;
            return scheduledMatchAttendance;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.ScheduledMatchAttendances
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(ScheduledMatchAttendanceErrors.ScheduledMatchAttendanceNotDeleted) : Result.Success();
        }

        public async Task<ScheduledMatchAttendance?> FindByPlayerAndMatchAsync(Guid playerId, Guid scheduledMatchId)
            => await _dbContext.ScheduledMatchAttendances
                .FirstOrDefaultAsync(a => a.PlayerId == playerId && a.ScheduledMatchId == scheduledMatchId);

        public async Task DeleteByPlayerAndMatchAsync(Guid playerId, Guid scheduledMatchId)
            => await _dbContext.ScheduledMatchAttendances
                .Where(a => a.PlayerId == playerId && a.ScheduledMatchId == scheduledMatchId)
                .ExecuteDeleteAsync();

        public async Task<Result> DeleteByScheduledMatchIdAsync(Guid scheduledMatchId)
        {
            await _dbContext.ScheduledMatchAttendances
                .Where(x => x.ScheduledMatchId == scheduledMatchId)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return Result.Success();
        }
    }
}
