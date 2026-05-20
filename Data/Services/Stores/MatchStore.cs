using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class MatchStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<MatchRecord> _matchRecordValidator;

        public MatchStore(MatchTrackerDbContext dbContext, IValidator<MatchRecord> matchRecordValidator)
        {
            _dbContext = dbContext;
            _matchRecordValidator = matchRecordValidator;
        }

        public async Task<List<MatchRecord>> GetAllMatchRecordsAsync()
            => await _dbContext.MatchRecords
                .Include(mr => mr.PlayingField)
                .Include(mr => mr.MatchVotes)
                .Include(mr => mr.MatchPlayers)
                .AsNoTracking()
                .ToListAsync();

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
                .Include(mr => mr.MatchPlayers).ThenInclude(mp => mp.Player)
                .Include(mr => mr.MatchPlayers).ThenInclude(mp => mp.PlayerRating)
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
                return Result<Guid>.FromResult(validationResult);

            _dbContext.MatchRecords.Add(matchRecord);
            await _dbContext.SaveChangesAsync();

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
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        private MatchRecord UpdateMatchRecord(IMatchRecord model, MatchRecord matchRecord)
        {
            matchRecord.WasMatchHeld = model.WasMatchHeld;
            matchRecord.MatchHeld = model.MatchHeld;
            matchRecord.PlayingFieldId = model.PlayingFieldId;
            matchRecord.GoalsTeamA = model.GoalsTeamA;
            matchRecord.GoalsTeamB = model.GoalsTeamB;
            return matchRecord;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.MatchRecords
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(MatchRecordErrors.MatchRecordNotDeleted) : Result.Success();
        }
    }
}
