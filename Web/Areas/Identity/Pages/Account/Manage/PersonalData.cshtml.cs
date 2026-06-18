#nullable disable
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public PersonalDataModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public bool IsGoogleUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            IsGoogleUser = (await _userManager.GetLoginsAsync(user))
                .Any(l => l.LoginProvider == "Google");
            return Page();
        }
    }
}
