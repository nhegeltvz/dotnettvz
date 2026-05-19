using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PlayerStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<Player> _playerValidator;

        public PlayerStore(MatchTrackerDbContext dbContext, IValidator<Player> playerValidator)
        {
            _dbContext = dbContext;
            _playerValidator = playerValidator;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
            => await _dbContext.Players
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Player>> GetPlayersByIdsAsync(IEnumerable<Guid> playerIds)
            => await _dbContext.Players
            .Where(player => playerIds.Contains(player.Id))
            .AsNoTracking().ToListAsync();

        public async Task<Result<Player>> FindByIdAsync(Guid id)
        {
            var player = await _dbContext.Players
                .Include(p => p.MatchPlayers)
                .Include(p => p.RatingsReceived)
                .FirstOrDefaultAsync(p => p.Id == id);

            return player != null
                ? Result<Player>.Success(player)
                : Result<Player>.Failure(PlayerErrors.PlayerNotFound);
        }

        public async Task<Result<Guid>> CreatePlayer(IPlayer model)
        {
            var player = new Player();
            UpdatePlayer(model, player);
            var validationResult = _playerValidator.Validate(player);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(player.Id);
        }

        public async Task<Result> UpdatePlayer(IPlayer model)
        {
            var foundPlayerResult = await FindByIdAsync(model.Id);

            if (!foundPlayerResult.IsSuccess || foundPlayerResult.Value == null)
                return foundPlayerResult;

            var player = foundPlayerResult.Value;
            UpdatePlayer(model, player);
            var validationResult = _playerValidator.Validate(player);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(PlayerErrors.PlayerNotUpdated);
        }

        private Player UpdatePlayer(IPlayer model, Player player)
        {
            player.Username = model.Username;
            player.Email = model.Email;
            player.Bio = model.Bio;
            player.PreferredPosition = model.PreferredPosition;
            player.Age = model.Age;
            return player;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.Players
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PlayerErrors.PlayerNotDeleted) : Result.Success();
        }

        public async Task<List<Player>> SearchByUsernameAsync(string term)
            => await _dbContext.Players
                .Where(player => EF.Functions.Like(player.Username, $"%{term}%"))
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<string>> SearchUsernamesAsync(string term)
            => await _dbContext.Players
                .Where(player => EF.Functions.Like(player.Username, $"%{term}%"))
                .Select(player => player.Username)
                .AsNoTracking()
                .ToListAsync();
    }
}
