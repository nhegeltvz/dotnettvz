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
                .Include(party => party.Members)
                .AsNoTracking()
                .ToListAsync();

        public IQueryable<Party> QueryPartiesAsync()
            => _dbContext.Parties
                .Include(party => party.PlayerCreated)
                    .ThenInclude(p => p.User)
                .Include(party => party.Members)
                    .ThenInclude(m => m.User)
                .Include(party => party.PreferredPlayingDates)
                .Include(party => party.ScheduledMatch)
                    .ThenInclude(match => match.PlayingField)
                .Include(party => party.ScheduledMatch)
                    .ThenInclude(match => match.ScheduledMatchAttendances)
                        .ThenInclude(attendance => attendance.Player)
                .AsNoTracking();

        public async Task<List<PartyListDto>> GetPartiesForTableAsync()
            => await _dbContext.Parties
            .Select(party => new PartyListDto
            {
                Id = party.Id,
                PlayerCreatedId = party.PlayerCreatedId,
                PartyDescription = party.PartyDescription,
                DateCreated = party.DateCreated,
                PlayerCreatedUsername = party.PlayerCreated.User.UserName ?? string.Empty,
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
                    .ThenInclude(p => p.User)
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Include(p => p.PreferredPlayingDates)
                .Include(p => p.ScheduledMatch)
                    .ThenInclude(sm => sm.PlayingField)
                        .ThenInclude(pf => pf.Images)
                .Include(p => p.ScheduledMatch)
                    .ThenInclude(sm => sm.ScheduledMatchAttendances)
                        .ThenInclude(a => a.Player)
                .Include(p => p.ScheduledMatch)
                    .ThenInclude(sm => sm.MatchRecord)
                        .ThenInclude(mr => mr.MatchPlayers)
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
            return Result.Success();
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

        public async Task SyncMembersAsync(Guid partyId, List<Guid> memberIds)
        {
            var party = await _dbContext.Parties
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == partyId);

            if (party == null)
                return;

            var players = await _dbContext.Players
                .Where(p => memberIds.Contains(p.Id))
                .ToListAsync();

            party.Members.Clear();
            foreach (var player in players)
                party.Members.Add(player);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Result> DeleteByIdAsync(Guid id)
        {
            var entity = await _dbContext.Parties.FindAsync(id);
            if (entity is null) return Result.Failure(PartyErrors.PartyNotDeleted);
            _dbContext.Parties.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
    }
}
