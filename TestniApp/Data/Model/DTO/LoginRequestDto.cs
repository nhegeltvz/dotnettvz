using System.ComponentModel.DataAnnotations;

namespace Data.Model.DTO
{
    public class LoginRequestDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
