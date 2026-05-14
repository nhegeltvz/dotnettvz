using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class PlayerStore
    {
        private readonly MatchTrackerDbContext _dbContext;

        public PlayerStore(MatchTrackerDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _dbContext.Players
                .Include(player => player.MatchPlayers)
                .Include(player => player.RatingsReceived)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Player?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Players
                        .Include(player => player.MatchPlayers)
                         .Include(player => player.RatingsReceived)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
