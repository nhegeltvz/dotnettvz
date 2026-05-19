using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class StadiumStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<PlayingField> _playingFieldValidator;

        public StadiumStore(MatchTrackerDbContext dbContext, IValidator<PlayingField> playingFieldValidator)
        {
            _dbContext = dbContext;
            _playingFieldValidator = playingFieldValidator;
        }

        public async Task<List<PlayingField>> GetAllStadiumsAsync()
            => await _dbContext.PlayingFields
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<PlayingField>> FindByIdAsync(Guid id)
        {
            var playingField = await _dbContext.PlayingFields
                .Include(pf => pf.MatchRecords)
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
                return Result<Guid>.FromResult(validationResult);

            _dbContext.PlayingFields.Add(playingField);
            await _dbContext.SaveChangesAsync();

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
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(PlayingFieldErrors.PlayingFieldNotUpdated);
        }

        private PlayingField UpdatePlayingField(IPlayingField model, PlayingField playingField)
        {

            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude;
            playingField.Latitude = model.Latitude;
            playingField.Image = model.Image;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = model.SurfaceType;
            return playingField;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.PlayingFields
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PlayingFieldErrors.PlayingFieldNotDeleted) : Result.Success();
        }

        public async Task<List<PlayingField>> SearchByNameAsync(string search)
        {
            return await _dbContext.PlayingFields.Where(x => x.Name.Contains(search)).AsNoTracking().ToListAsync();
        }
    }
}
