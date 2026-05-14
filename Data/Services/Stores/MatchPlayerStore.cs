using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class MatchPlayerStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<MatchPlayer> _matchPlayerValidator;

        public MatchPlayerStore(MatchTrackerDbContext dbContext, IValidator<MatchPlayer> matchPlayerValidator)
        {
            _dbContext = dbContext;
            _matchPlayerValidator = matchPlayerValidator;
        }

        public async Task<List<MatchPlayer>> GetAllMatchPlayersAsync()
            => await _dbContext.MatchPlayers
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<MatchPlayer>> FindByIdAsync(Guid id)
        {
            var matchPlayer = await _dbContext.MatchPlayers.FindAsync(id);
            return matchPlayer != null ? Result<MatchPlayer>.Success(matchPlayer) : Result<MatchPlayer>.Failure(MatchPlayerErrors.MatchPlayerNotFound);
        }

        public async Task<Result<Guid>> CreateMatchPlayer(IMatchPlayer model)
        {
            var matchPlayer = new MatchPlayer();
            UpdateMatchPlayer(model, matchPlayer);
            var validationResult = _matchPlayerValidator.Validate(matchPlayer);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.MatchPlayers.Add(matchPlayer);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(matchPlayer.Id);
        }

        public async Task<Result> UpdateMatchPlayer(IMatchPlayer model)
        {
            var foundMatchPlayerResult = await FindByIdAsync(model.Id);

            if (!foundMatchPlayerResult.IsSuccess || foundMatchPlayerResult.Value == null)
                return foundMatchPlayerResult;

            var matchPlayer = foundMatchPlayerResult.Value;
            UpdateMatchPlayer(model, matchPlayer);
            var validationResult = _matchPlayerValidator.Validate(matchPlayer);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(MatchPlayerErrors.MatchPlayerNotUpdated);
        }

        private MatchPlayer UpdateMatchPlayer(IMatchPlayer model, MatchPlayer matchPlayer)
        {
            matchPlayer.PlayerId = model.PlayerId;
            matchPlayer.Team = model.Team;
            matchPlayer.MatchRecordId = model.MatchRecordId;
            matchPlayer.Goals = model.Goals;
            matchPlayer.Assists = model.Assists;
            return matchPlayer;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.MatchPlayers
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(MatchPlayerErrors.MatchPlayerNotDeleted) : Result.Success();
        }
    }
}
