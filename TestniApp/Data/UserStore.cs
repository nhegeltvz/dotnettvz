using Data.Data;
using Data.Model;
using Data.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UserStore
    {
        private readonly TicketDbContext _dbContext;

        public UserStore(TicketDbContext dbContext) => _dbContext = dbContext;

        public async Task CreateUser(IUser model)
        {
            var user = new User();

            user.Email = model.Email;
            user.PasswordHash = model.PasswordHash;
            user.DisplayName = model.DisplayName;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<User?> FindByEmailAsync(string email)
            =>  await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == user.Email);
    }
}
