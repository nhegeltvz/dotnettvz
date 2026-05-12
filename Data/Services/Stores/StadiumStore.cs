using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Stores
{
    public class StadiumStore
    {
        private readonly MatchTrackerDbContext _dbContext;

        public StadiumStore(MatchTrackerDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<PlayingField>> GetAllStadiumsAsync()
            => await _dbContext.PlayingFields
                .AsNoTracking()
                .ToListAsync();

        public async Task<PlayingField?> FindByIdAsync(Guid id)
        {
            return await _dbContext.PlayingFields
                        .Include(field => field.MatchRecords)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(f => f.Id == id);
        }
        
    }
}
