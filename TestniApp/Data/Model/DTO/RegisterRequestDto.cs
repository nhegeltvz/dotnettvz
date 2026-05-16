using System.ComponentModel.DataAnnotations;

namespace Data.Model.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(20)]
        public string DisplayName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(32)]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$", ErrorMessage = "Password must be 8-32 characters and contain upper, lower, digit, and special char.")]
        public string Password { get; set; } = string.Empty;
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
