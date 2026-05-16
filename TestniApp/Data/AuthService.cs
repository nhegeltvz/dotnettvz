using Data.Model;
using Data.Model.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Data
{
    public class AuthService
    {
        private readonly UserStore _userStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserStore userStore, IHttpContextAccessor httpContextAccessor)
        {
            _userStore = userStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LoginAsync(LoginRequestDto loginRequest)
        {
            var passwordHasher = new PasswordHasher<User>();
            var foundUser = _userStore.FindByEmailAsync(loginRequest.Email);

            if (foundUser is null)
                return;

            var user = foundUser.Result;

            var passwordVerficationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

            if (passwordVerficationResult != PasswordVerificationResult.Success)
                return;

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequest)
        {
            var passwordHasher = new PasswordHasher<User>();
            var user = new User();

            user.Id = Guid.NewGuid().ToString();
            user.DisplayName = registerRequest.DisplayName;
            user.Email = registerRequest.Email;
            user.PasswordHash = passwordHasher.HashPassword(user, registerRequest.Password);

            await _userStore.CreateUser(user);

            var claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, user.DisplayName), 
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id) 
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }
    }
}
