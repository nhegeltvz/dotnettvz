using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.Party;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Validation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PartyStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IValidator<Party> _partyValidator;

        public PartyStore(MatchTrackerDbContext dbContext, IValidator<Party> partyValidator)
        {
            _dbContext = dbContext;
            _partyValidator = partyValidator;
        }

        public async Task<List<Party>> GetAllPartiesAsync()
            => await _dbContext.Parties
                .Include(party => party.PlayerCreated)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<PartyListDto>> GetPartiesForTableAsync()
            => await _dbContext.Parties
            .Select(party => new PartyListDto
            {
                Id = party.Id,
                PlayerCreatedId = party.PlayerCreatedId,
                PartyDescription = party.PartyDescription,
                DateCreated = party.DateCreated,
                PlayerCreatedUsername= party.PlayerCreated.Username,
                NumberOfMembers = party.Members.Count,
                MaxMembers = party.MaxMembers,
                PreferredLocations = party.PreferredLocations
            })
                .AsNoTracking()
                .ToListAsync();

        public async Task<Result<Party>> FindByIdAsync(Guid id)
        {
            var party = await _dbContext.Parties
                .Include(p => p.PlayerCreated)
                .Include(p => p.Members)
                .Include(p => p.PreferredPlayingDates)
                .FirstOrDefaultAsync(p => p.Id == id);

            return party != null
                ? Result<Party>.Success(party)
                : Result<Party>.Failure(PartyErrors.PartyNotFound);
        }

        public async Task<Result<Guid>> CreateParty(IParty model)
        {
            var party = new Party();
            UpdateParty(model, party);
            var validationResult = _partyValidator.Validate(party);
            if (!validationResult.IsSuccess)
                return Result<Guid>.FromResult(validationResult);

            _dbContext.Parties.Add(party);
            await _dbContext.SaveChangesAsync();

            return Result<Guid>.Success(party.Id);
        }

        public async Task<Result> UpdateParty(IParty model)
        {
            var foundPartyResult = await FindByIdAsync(model.Id);

            if (!foundPartyResult.IsSuccess || foundPartyResult.Value == null)
                return foundPartyResult;

            var party = foundPartyResult.Value;
            UpdateParty(model, party);
            var validationResult = _partyValidator.Validate(party);

            if (!validationResult.IsSuccess)
                return validationResult;

            var rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected != 0 ? Result.Success() : Result.Failure(PartyErrors.PartyNotUpdated);
        }

        private Party UpdateParty(IParty model, Party party)
        {
            party.PlayerCreatedId = model.PlayerCreatedId;
            party.DateCreated = model.DateCreated;
            party.MaxMembers = model.MaxMembers;
            party.PartyDescription = model.PartyDescription;
            party.PreferredLocations = model.PreferredLocations;
            return party;
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var rowsAffected = await _dbContext.Parties
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);

            return rowsAffected == 0 ? Result.Failure(PartyErrors.PartyNotDeleted) : Result.Success();
        }
    }
}
