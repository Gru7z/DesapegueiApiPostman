
using System.Security.Claims;
using System.Text;
using Desapeguei.Api.Data;
using Desapeguei.Api.Repositories;
using Desapeguei.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada.");

builder.Services.AddDbContext<ProdutoContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Produtos
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

// Usuários e autenticação
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<AuthService>();

// Vendas
builder.Services.AddScoped<VendaRepository>();
builder.Services.AddScoped<VendaService>();

// JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
            };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var idTexto = context.Principal?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(idTexto, out var id))
                {
                    context.Fail("Token inválido.");
                    return;
                }

                var db = context.HttpContext.RequestServices
                    .GetRequiredService<ProdutoContext>();

                var usuario = await db.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Id == id && u.Ativo)
                    .Select(u => new { u.Admin })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    context.Fail(
                        "Usuário não existe ou está desativado.");
                    return;
                }

                if (usuario.Admin &&
                    context.Principal?.Identity is ClaimsIdentity identity)
                {
                    identity.AddClaim(
                        new Claim(ClaimTypes.Role, "Admin"));
                }
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS para o site local
builder.Services.AddCors(options =>
{
    options.AddPolicy("Site", policy =>
    {
        policy
            .WithOrigins("https://localhost:7276")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Site");
app.UseAuthentication();
app.UseAuthorization();

// Cria o administrador da API, se necessário
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ProdutoContext>();

    await DbDefineAdmin.SeedAdminAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
