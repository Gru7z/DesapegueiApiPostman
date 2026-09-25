using Desapeguei.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Web.Areas.Identity.Pages.Account.Manage
{
    public class DadosPessoaisModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DadosPessoaisModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }

        public async Task OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario != null)
            {
                Email = usuario.Email;
                Telefone = usuario.PhoneNumber;

                var dados = await _context.DadosPessoais
                    .FirstOrDefaultAsync(x => x.UserId == usuario.Id);

                if (dados != null)
                {
                    Nome = dados.Nome;
                    Endereco = dados.Endereco;
                }
            }
        }
    }
}