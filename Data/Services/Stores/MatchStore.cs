using Data.Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Data.Services.Stores
{
    public class MatchStore
    {
        private readonly MatchTrackerDbContext _dbContext;
        public MatchStore(MatchTrackerDbContext dbContext) => _dbContext = dbContext;
        public async Task<List<MatchRecord>> GetMatchesAsync()
            => await _dbContext.MatchRecords
                .Include(match => match.PlayingField)
                .AsNoTracking()
                .ToListAsync();


        public async Task<MatchRecord?> FindByIdAsync(Guid id)
            =>await _dbContext.MatchRecords
                    .Include(match => match.PlayingField)
                    .Include(match => match.MatchPlayers)
                    .ThenInclude(matchPlayer => matchPlayer.Player)
                    .Include(match => match.MatchVotes)
                    .ThenInclude(matchVote => matchVote.Player)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == id);
        }
}
