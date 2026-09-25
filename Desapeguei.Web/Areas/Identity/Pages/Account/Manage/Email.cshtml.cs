using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desapeguei.Web.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string? Email { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            public string? NewEmail { get; set; }
        }

        public async Task OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario != null)
            {
                Email = await _userManager.GetEmailAsync(usuario);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(Input.NewEmail))
            {
                await _userManager.SetEmailAsync(usuario, Input.NewEmail);
                await _userManager.SetUserNameAsync(usuario, Input.NewEmail);
            }

            return RedirectToPage("./Email");
        }
    }
}