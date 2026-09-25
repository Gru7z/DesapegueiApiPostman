using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desapeguei.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        public string? Email { get; set; }

        public void OnGet()
        {
            Email = User.Identity?.Name;
        }
    }
}