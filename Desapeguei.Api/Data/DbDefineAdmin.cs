using Desapeguei.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Desapeguei.Api.Data
{
    public static class DbDefineAdmin
    {
        private const string AdminEmail = "admin@email.com";

        public static async Task SeedAdminAsync(ProdutoContext db)
        {
            // Já existe algum admin? Então não faz nada.
            if (await db.Usuarios.AnyAsync(u => u.Admin))
                return;

            // Se já existe uma conta com esse e-mail, só promove
            var admin = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == AdminEmail);

            if (admin != null)
            {
                admin.Admin = true;
                admin.Ativo = true;
            }
            else
            {
                // Senha inicial: variável de ambiente ADMIN_SENHA, ou "Admin@123" se não existir
                var senha = Environment.GetEnvironmentVariable("ADMIN_SENHA") ?? "Admin@123";

                admin = new Usuario
                {
                    Nome = "Administrador",
                    Email = AdminEmail,
                    Telefone = "00000000000",
                    Admin = true,
                    Ativo = true
                };
                admin.Senha = new PasswordHasher<Usuario>().HashPassword(admin, senha);

                db.Usuarios.Add(admin);
            }

            await db.SaveChangesAsync();
        }
    }
}