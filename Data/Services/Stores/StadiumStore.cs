using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Services.Stores
{
    public class StadiumStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<PlayingField> _playingFieldValidator;
        private readonly ILogger<StadiumStore> _logger;
        private readonly CurrentUserService _currentUser;

        public StadiumStore(MatchTrackerDbContext dbContext, IValidator<PlayingField> playingFieldValidator, ILogger<StadiumStore> logger, CurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _playingFieldValidator = playingFieldValidator;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<List<PlayingField>> GetAllStadiumsAsync()
            => await _dbContext.PlayingFields
                .Include(pf => pf.Images)
                .AsNoTracking()
                .ToListAsync();

        public IQueryable<PlayingField> QueryPlayingFieldsAsync()
            => _dbContext.PlayingFields.AsNoTracking();

        public async Task<Result<PlayingField>> FindByIdAsync(Guid id)
        {
            var playingField = await _dbContext.PlayingFields
                .Include(pf => pf.MatchRecords)
                .Include(pf => pf.Images)
                .FirstOrDefaultAsync(pf => pf.Id == id);

            return playingField != null
                ? Result<PlayingField>.Success(playingField)
                : Result<PlayingField>.Failure(PlayingFieldErrors.PlayingFieldNotFound);
        }

        public async Task<Result<Guid>> CreatePlayingField(IPlayingField model)
        {
            var playingField = new PlayingField();
            UpdatePlayingField(model, playingField);
            var validationResult = _playingFieldValidator.Validate(playingField);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Playing field creation failed by {UserId}, validation errors: {Errors}",
                    _currentUser.Id, string.Join(", ", validationResult.Errors));
                return Result<Guid>.FromResult(validationResult);
            }

            _dbContext.PlayingFields.Add(playingField);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Playing field created: {FieldId} by {UserId}, Data: {Model}",
                playingField.Id, _currentUser.Id, model);
            return Result<Guid>.Success(playingField.Id);
        }

        public async Task<Result> UpdatePlayingField(IPlayingField model)
        {
            var foundPlayingFieldResult = await FindByIdAsync(model.Id);

            if (!foundPlayingFieldResult.IsSuccess || foundPlayingFieldResult.Value == null)
                return foundPlayingFieldResult;

            var playingField = foundPlayingFieldResult.Value;
            UpdatePlayingField(model, playingField);
            var validationResult = _playingFieldValidator.Validate(playingField);

            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Playing field update failed for {FieldId} by {UserId}, validation errors: {Errors}",
                    model.Id, _currentUser.Id, string.Join(", ", validationResult.Errors));
                return validationResult;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Playing field updated: {FieldId} by {UserId}, Data: {Model}",
                model.Id, _currentUser.Id, model);
            return Result.Success();
        }

        private PlayingField UpdatePlayingField(IPlayingField model, PlayingField playingField)
        {
            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude;
            playingField.Latitude = model.Latitude;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = model.SurfaceType;
            return playingField;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var entity = await _dbContext.PlayingFields.FindAsync(id);
            if (entity is null) return Result.Failure(PlayingFieldErrors.PlayingFieldNotDeleted);
            _dbContext.PlayingFields.Remove(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Playing field deleted: {FieldId} by {UserId}", id, _currentUser.Id);
            return Result.Success();
        }

        public async Task<List<PlayingField>> SearchByNameAsync(string search)
        {
            return await _dbContext.PlayingFields.Where(x => x.Name.Contains(search)).AsNoTracking().ToListAsync();
        }

        public async Task AddImageResourceAsync(ImageResource image)
        {
            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LinkImagesToFieldAsync(Guid playingFieldId, List<Guid> imageIds)
        {
            await _dbContext.Images
                .Where(img => imageIds.Contains(img.Id) && img.PlayingFieldId == null)
                .ExecuteUpdateAsync(s => s.SetProperty(img => img.PlayingFieldId, playingFieldId));
        }

        public async Task<Result<ImageResource>> GetImageByIdAsync(Guid imageId)
        {
            var image = await _dbContext.Images.FindAsync(imageId);
            return image != null
                ? Result<ImageResource>.Success(image)
                : Result<ImageResource>.Failure(ImageResourceErrors.ImageNotFound);
        }

        public async Task<Result> RemoveImageAsync(Guid imageId)
        {
            var rowsAffected = await _dbContext.Images
                .Where(img => img.Id == imageId)
                .ExecuteDeleteAsync();
            return rowsAffected > 0 ? Result.Success() : Result.Failure(ImageResourceErrors.ImageNotDeleted);
        }
    }
}
