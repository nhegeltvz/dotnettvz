using Data.Models;
using System.Linq.Expressions;

namespace Data.Dto.CRUD.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public static Expression<Func<AppUser, UserDto>> ToDto()
        {
            return user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}
