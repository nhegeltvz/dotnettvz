using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PartyStore
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PartyStore(MatchTrackerDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<Party>> GetPartiesAsync()
        {
            return await _dbContext.Parties
                .Include(party => party.PlayerCreated)
                .Include(party => party.Members)
                .Include(party => party.PreferredPlayingDates)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Party?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Parties
                    .Include(party => party.PlayerCreated)
                    .Include(party => party.Members)
                    .Include(party => party.PreferredPlayingDates)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
