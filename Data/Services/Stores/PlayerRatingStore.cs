using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PlayerRatingStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<PlayerRating> _playerRatingValidator;

        public PlayerRatingStore(MatchTrackerDbContext dbContext, IValidator<PlayerRating> playerRatingValidator)
        {
            _dbContext = dbContext;
            _playerRatingValidator = playerRatingValidator;
        }

        public async Task<List<PlayerRating>> GetAllPlayerRatingsAsync()
            => await _dbContext.PlayerRatings
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<PlayerRating>> FindByIdAsync(Guid id)
        {
            var playerRating = await _dbContext.PlayerRatings.FindAsync(id);
            return playerRating != null ? Result<PlayerRating>.Success(playerRating) : Result<PlayerRating>.Failure(PlayerRatingErrors.PlayerRatingNotFound);
        }

        public async Task<Result<Guid>> CreatePlayerRating(IPlayerRating model)
        {
            var playerRating = new PlayerRating();
            UpdatePlayerRating(model, playerRating);
            var validationResult = _playerRatingValidator.Validate(playerRating);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.PlayerRatings.Add(playerRating);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(playerRating.Id);
        }

        public async Task<Result> UpdatePlayerRating(IPlayerRating model)
        {
            var foundPlayerRatingResult = await FindByIdAsync(model.Id);

            if (!foundPlayerRatingResult.IsSuccess || foundPlayerRatingResult.Value == null)
                return foundPlayerRatingResult;

            var playerRating = foundPlayerRatingResult.Value;
            UpdatePlayerRating(model, playerRating);
            var validationResult = _playerRatingValidator.Validate(playerRating);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        private PlayerRating UpdatePlayerRating(IPlayerRating model, PlayerRating playerRating)
        {
            playerRating.MatchPlayerId = model.MatchPlayerId;
            playerRating.PlayerGivingRatingId = model.PlayerGivingRatingId;
            playerRating.PlayerReceivingRatingId = model.PlayerReceivingRatingId;
            playerRating.Rating = model.Rating;
            return playerRating;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.PlayerRatings
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PlayerRatingErrors.PlayerRatingNotDeleted) : Result.Success();
        }

        public async Task<Result> DeleteByMatchPlayerIdsAsync(IEnumerable<Guid> matchPlayerIds)
        {
            var ids = matchPlayerIds.ToList();
            if (!ids.Any())
            {
                return Result.Success();
            }

            await _dbContext.PlayerRatings
                .Where(x => ids.Contains(x.MatchPlayerId))
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return Result.Success();
        }
    }
}
