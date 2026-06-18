#nullable disable
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public DeletePersonalDataModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public bool RequirePassword { get; set; }
        public bool IsGoogleUser { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            RequirePassword = await _userManager.HasPasswordAsync(user);
            IsGoogleUser = (await _userManager.GetLoginsAsync(user))
                .Any(l => l.LoginProvider == "Google");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            RequirePassword = await _userManager.HasPasswordAsync(user);
            IsGoogleUser = (await _userManager.GetLoginsAsync(user))
                .Any(l => l.LoginProvider == "Google");

            if (RequirePassword)
            {
                if (string.IsNullOrEmpty(Input?.Password) ||
                    !await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError("Input.Password", "Lozinka nije ispravna.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            await _signInManager.SignOutAsync();
            return Redirect("/home");
        }
    }
}
