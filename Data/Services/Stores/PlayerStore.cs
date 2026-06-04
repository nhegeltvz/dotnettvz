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
        private readonly IValidator<AppUser> _playerValidator;

        public PlayerStore(MatchTrackerDbContext dbContext, IValidator<AppUser> playerValidator)
        {
            _dbContext = dbContext;
            _playerValidator = playerValidator;
        }

        public async Task<List<AppUser>> GetAllPlayersAsync()
            => await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();

        public IQueryable<AppUser> QueryPlayersAsync()
            => _dbContext.Users;

        public async Task<List<AppUser>> GetPlayersByIdsAsync(IEnumerable<Guid> playerIds)
            => await _dbContext.Users
                .Where(player => playerIds.Contains(player.Id))
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<AppUser>> FindByIdAsync(Guid id)
        {
            var player = await _dbContext.Users
                .Include(p => p.MatchPlayers)
                .Include(p => p.RatingsReceived)
                .FirstOrDefaultAsync(p => p.Id == id);

            return player != null
                ? Result<AppUser>.Success(player)
                : Result<AppUser>.Failure(PlayerErrors.PlayerNotFound);
        }

        public async Task<Result<Guid>> CreatePlayer(IPlayer model)
        {
            var player = new AppUser();
            UpdatePlayer(model, player);
            var validationResult = _playerValidator.Validate(player);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.Users.Add(player);
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

        private AppUser UpdatePlayer(IPlayer model, AppUser player)
        {
            player.Username = model.Username;
            player.Bio = model.Bio;
            player.PreferredPosition = model.PreferredPosition;
            player.Age = model.Age;
            return player;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.Users
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PlayerErrors.PlayerNotDeleted) : Result.Success();
        }

        public async Task<List<AppUser>> SearchByUsernameAsync(string term)
            => await _dbContext.Users
                .Where(player => EF.Functions.Like(player.Username, $"%{term}%"))
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<string>> SearchUsernamesAsync(string term)
            => await _dbContext.Users
                .Where(player => EF.Functions.Like(player.Username, $"%{term}%"))
                .Select(player => player.Username)
                .AsNoTracking()
                .ToListAsync();
    }
}
