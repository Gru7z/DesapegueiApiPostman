using Microsoft.AspNetCore.Identity;

namespace Desapeguei.Web.Data
{
    // Responsável por garantir que o papel "Admin" e o usuário administrador
    // padrão do site existam sempre que a aplicação for iniciada.
    public static class Admin
    {
        public const string AdminRole = "Admin";
        public const string AdminEmail = "admin@desapeguei.com";
        public const string AdminSenha = "Admin@12345";

        public static async Task SeedAdminAsync(IServiceProvider servicos)
        {
            var roleManager = servicos.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = servicos.GetRequiredService<UserManager<IdentityUser>>();

            // Garante que o papel "Admin" exista
            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            // Garante que o usuário administrador exista
            var admin = await userManager.FindByEmailAsync(AdminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(admin, AdminSenha);

                if (!resultado.Succeeded)
                {
                    // Se por algum motivo a criação falhar, não trava a aplicação,
                    // mas evita continuar tentando associar o papel a um usuário inexistente.
                    return;
                }
            }

            // Garante que o usuário administrador esteja no papel "Admin"
            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }
    }
}
