using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Services.Stores
{
    public class MatchStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<MatchRecord> _matchRecordValidator;
        private readonly ILogger<MatchStore> _logger;
        private readonly CurrentUserService _currentUser;

        public MatchStore(MatchTrackerDbContext dbContext, IValidator<MatchRecord> matchRecordValidator, ILogger<MatchStore> logger, CurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _matchRecordValidator = matchRecordValidator;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<List<MatchRecord>> GetAllMatchRecordsAsync()
            => await _dbContext.MatchRecords
                .Include(mr => mr.PlayingField)
                .Include(mr => mr.MatchVotes)
                .Include(mr => mr.MatchPlayers)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<MatchRecord>> GetMatchRecordsByDateAsync(DateOnly date)
        {
            var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end   = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            return await _dbContext.MatchRecords
                .Include(mr => mr.PlayingField)
                .Include(mr => mr.MatchVotes)
                .Include(mr => mr.MatchPlayers)
                .Where(mr => mr.MatchHeld >= start && mr.MatchHeld <= end)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MatchRecord>> GetMatchRecordsByRangeAsync(DateTime from, DateTime to)
            => await _dbContext.MatchRecords
                .Include(mr => mr.PlayingField)
                .Include(mr => mr.MatchVotes)
                .Include(mr => mr.MatchPlayers)
                .Where(mr => mr.MatchHeld >= from && mr.MatchHeld <= to)
                .OrderByDescending(mr => mr.MatchHeld)
                .AsNoTracking()
                .ToListAsync();

        public async Task<(int Last7, int Last14, int Last30)> GetPeriodMatchCountsAsync()
        {
            var now = DateTime.UtcNow;
            var d7  = now.AddDays(-7);
            var d14 = now.AddDays(-14);
            var d30 = now.AddDays(-30);
            var last7  = await _dbContext.MatchRecords.CountAsync(m => m.MatchHeld >= d7);
            var last14 = await _dbContext.MatchRecords.CountAsync(m => m.MatchHeld >= d14);
            var last30 = await _dbContext.MatchRecords.CountAsync(m => m.MatchHeld >= d30);
            return (last7, last14, last30);
        }

        public IQueryable<MatchRecord> QueryMatchesAsync()
                 => _dbContext.MatchRecords;

        public async Task<List<MatchRecordListDto>> GetMatchRecordsForTableAsync()
            => await _dbContext.MatchRecords
                .Select(record => new MatchRecordListDto
                {
                    Id = record.Id,
                    GoalsTeamA = record.GoalsTeamA,
                    GoalsTeamB = record.GoalsTeamB,
                    PlayingFieldId = record.PlayingFieldId,
                    PlayingFieldName = record.PlayingField.Name,
                    MatchHeld = record.MatchHeld,
                    WasMatchHeld = record.WasMatchHeld,
                })
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<MatchRecord>> FindByIdAsync(Guid id)
        {
            var matchRecord = await _dbContext.MatchRecords
                .Include(mr => mr.PlayingField)
                .Include(mr => mr.MatchPlayers).ThenInclude(mp => mp.Player).ThenInclude(p => p.User)
                .Include(mr => mr.MatchPlayers).ThenInclude(mp => mp.PlayerRating).ThenInclude(pr => pr.PlayerGivingRating).ThenInclude(p => p.User)
                .Include(mr => mr.MatchVotes).ThenInclude(mv => mv.Player)
                .FirstOrDefaultAsync(mr => mr.Id == id);

            return matchRecord != null
                ? Result<MatchRecord>.Success(matchRecord)
                : Result<MatchRecord>.Failure(MatchRecordErrors.MatchRecordNotFound);
        }

        public async Task<Result<Guid>> CreateMatchRecord(IMatchRecord model)
        {
            var matchRecord = new MatchRecord();
            UpdateMatchRecord(model, matchRecord);
            var validationResult = _matchRecordValidator.Validate(matchRecord);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Match record creation failed by {UserId}, validation errors: {Errors}",
                    _currentUser.Id, string.Join(", ", validationResult.Errors));
                return Result<Guid>.FromResult(validationResult);
            }

            _dbContext.MatchRecords.Add(matchRecord);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Match record created: {MatchId} by {UserId}, Data: {Model}",
                matchRecord.Id, _currentUser.Id, model);
            return Result<Guid>.Success(matchRecord.Id);
        }

        public async Task<Result> UpdateMatchRecord(IMatchRecord model)
        {
            var foundMatchRecordResult = await FindByIdAsync(model.Id);

            if (!foundMatchRecordResult.IsSuccess || foundMatchRecordResult.Value == null)
                return foundMatchRecordResult;

            var matchRecord = foundMatchRecordResult.Value;
            UpdateMatchRecord(model, matchRecord);
            var validationResult = _matchRecordValidator.Validate(matchRecord);

            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Match record update failed for {MatchId} by {UserId}, validation errors: {Errors}",
                    model.Id, _currentUser.Id, string.Join(", ", validationResult.Errors));
                return validationResult;
            }

            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();
            _logger.LogInformation("Match record updated: {MatchId} by {UserId}, Data: {Model}",
                model.Id, _currentUser.Id, model);
            return Result.Success();
        }

        private MatchRecord UpdateMatchRecord(IMatchRecord model, MatchRecord matchRecord)
        {
            matchRecord.WasMatchHeld = model.WasMatchHeld;
            matchRecord.MatchHeld = model.MatchHeld;
            matchRecord.PlayingFieldId = model.PlayingFieldId;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;
            matchRecord.ScheduledMatchId = model.ScheduledMatchId;
            return matchRecord;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var entity = await _dbContext.MatchRecords.FindAsync(id);
            if (entity is null) return Result.Failure(MatchRecordErrors.MatchRecordNotDeleted);
            _dbContext.MatchRecords.Remove(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Match record deleted: {MatchId} by {UserId}", id, _currentUser.Id);
            return Result.Success();
        }
    }
}
