using Desapeguei.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Web.Data
{
    // Este é o "banco do usuário": contas do Identity (usuários e papéis)
    // e os dados pessoais complementares (nome, telefone, endereço).
    // Os produtos NÃO fazem mais parte deste banco — eles agora
    // pertencem à Desapeguei.Api.
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<DadosPessoais> DadosPessoais { get; set; } = null!;
    }
}
