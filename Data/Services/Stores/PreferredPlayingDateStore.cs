using Data.Data;
using Data.Data.Common;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PreferredPlayingDateStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<PreferredPlayingDate> _preferredPlayingDateValidator;

        public PreferredPlayingDateStore(MatchTrackerDbContext dbContext, IValidator<PreferredPlayingDate> preferredPlayingDateValidator)
        {
            _dbContext = dbContext;
            _preferredPlayingDateValidator = preferredPlayingDateValidator;
        }

        public async Task<List<PreferredPlayingDate>> GetAllPreferredPlayingDatesAsync()
            => await _dbContext.PreferredPlayingDates
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<PreferredPlayingDate>> FindByIdAsync(Guid id)
        {
            var preferredPlayingDate = await _dbContext.PreferredPlayingDates.FindAsync(id);
            return preferredPlayingDate != null ? Result<PreferredPlayingDate>.Success(preferredPlayingDate) : Result<PreferredPlayingDate>.Failure(PreferredPlayingDateErrors.PreferredPlayingDateNotFound);
        }

        public async Task<Result<Guid>> CreatePreferredPlayingDate(IPreferredPlayingDate model)
        {
            var preferredPlayingDate = new PreferredPlayingDate();
            UpdatePreferredPlayingDate(model, preferredPlayingDate);
            var validationResult = _preferredPlayingDateValidator.Validate(preferredPlayingDate);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.PreferredPlayingDates.Add(preferredPlayingDate);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(preferredPlayingDate.Id);
        }

        public async Task<Result> UpdatePreferredPlayingDate(IPreferredPlayingDate model)
        {
            var foundPreferredPlayingDateResult = await FindByIdAsync(model.Id);

            if (!foundPreferredPlayingDateResult.IsSuccess || foundPreferredPlayingDateResult.Value == null)
                return foundPreferredPlayingDateResult;

            var preferredPlayingDate = foundPreferredPlayingDateResult.Value;
            UpdatePreferredPlayingDate(model, preferredPlayingDate);
            var validationResult = _preferredPlayingDateValidator.Validate(preferredPlayingDate);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(PreferredPlayingDateErrors.PreferredPlayingDateNotUpdated);
        }

        private PreferredPlayingDate UpdatePreferredPlayingDate(IPreferredPlayingDate model, PreferredPlayingDate preferredPlayingDate)
        {
            preferredPlayingDate.PartyId = model.PartyId;
            preferredPlayingDate.Date = model.Date;
            return preferredPlayingDate;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.PreferredPlayingDates
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PreferredPlayingDateErrors.PreferredPlayingDateNotDeleted) : Result.Success();
        }
    }
}
