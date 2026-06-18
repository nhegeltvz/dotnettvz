#nullable disable
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string CurrentUsername { get; set; }
        public string Email { get; set; }
        public bool IsGoogleUser { get; set; }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Korisničko ime je obavezno.")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
            [Display(Name = "Korisničko ime")]
            public string Username { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            CurrentUsername = user.UserName;
            Email = user.Email;
            IsGoogleUser = (await _userManager.GetLoginsAsync(user))
                .Any(l => l.LoginProvider == "Google");
            Input = new InputModel { Username = CurrentUsername };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.Username != user.UserName)
            {
                if (await _userManager.FindByNameAsync(Input.Username) != null)
                {
                    ModelState.AddModelError("Input.Username", "Korisničko ime je već zauzeto.");
                    await LoadAsync(user);
                    return Page();
                }

                var result = await _userManager.SetUserNameAsync(user, Input.Username);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("Input.Username", error.Description);
                    await LoadAsync(user);
                    return Page();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profil je uspješno ažuriran.";
            return RedirectToPage();
        }
    }
}
