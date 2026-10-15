using System.Security.Claims;
using System.Text;
using Desapeguei.Api.Data;
using Desapeguei.Api.Repositories;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------- Banco ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ProdutoContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ---------- Produtos ----------
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

// ---------- Usuários / JWT ----------
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<AuthService>();

//------------ Vendas ----------
builder.Services.AddScoped<VendaRepository>();
builder.Services.AddScoped<VendaService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero, // expira no minuto exato
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var idTexto = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(idTexto, out var id))
                {
                    context.Fail("Token inválido.");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<ProdutoContext>();

                // Se o usuário foi apagado ou desativado, o token deixa de valer
                var usuario = await db.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Id == id && u.Ativo)
                    .Select(u => new { u.Admin })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    context.Fail("Usuário não existe ou está desativado.");
                    return;
                }

                // O papel vem SEMPRE do banco, não do token
                if (usuario.Admin && context.Principal?.Identity is ClaimsIdentity identity)
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ---------- Cria o admin automaticamente (se ainda não existir) ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProdutoContext>();
    await DbDefineAdmin.SeedAdminAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();   // primeiro: "quem é você?"
app.UseAuthorization();    // depois: "você pode entrar?"

app.MapControllers();

app.Run();