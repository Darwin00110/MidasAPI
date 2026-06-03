using MidasAPI;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MidasAPI.src.Application.UseCase;
using MidasAPI.src.Domain.Interfaces.UserUseCase_NoService;
using MidasAPI.src.Domain.Interfaces.IUserUseCase;
using MidasAPI.src.Infra.Services.Hash;
using MidasAPI.src.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;

var envPath = FindEnvFile();
if (envPath is not null)
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings__DefaultConnection=Server");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
}

var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtSecret) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddControllers();
builder.Services.AddScoped<ICPF_USER, CPF_USER>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<ITokenJWT, TokenJWT>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserUseCase_NoService, UserUseCase_NoService>();
builder.Services.AddScoped<IUserUseCase_WithService, UserUseCase_WithService>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITransacaoUseCase, TransacaoUseCase>();

builder.Services.AddScoped<IAdminUseCase_NoService, AdminUseCase_NoService>();
builder.Services.AddScoped<IAdminUseCase_WithService, AdminUseCase_WithService>();

builder.Services.AddScoped<IAccountsUseCase_NoService, AccountsUseCase_NoService>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
    };
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtivoApenas", policy => policy.RequireClaim("Status", "ATIVO"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IHashService>();

    await context.Database.MigrateAsync();

    if (!await context.Users.AnyAsync(u => u.Role == OptionsRole.ADMIN))
    {
        var senhaHash = await hasher.HashPassword("amora0909!");
        var DataUser = new User
        {
            Nome = "Banco_MidasAPI",
            Email = "Admin@gmail.com",
            CPF = "15385638406",
            PasswordHash = senhaHash,
            Role = OptionsRole.ADMIN,
            DataNascimento = new DateTime(2008, 01, 30),
            Status = OptionsStatus.ATIVO,
            CreatedAt = DateTime.Now,
            Telefone = "31987653872"
        };
        var DataAccount = new Accounts
        {
            ChavePix = "00000000000",
            User = DataUser,
            NumeroConta = "00000001",
            NumeroAgencia = "0001",
            Saldo = 1000000,
            TipoConta = OptionsTipoDaConta.CONTA_SALARIO,
            Status = OptionsStatus.ATIVO,
            CriadoEm = DateTime.Now,
        };
        context.Users.Add(DataUser);
        context.Accounts.Add(DataAccount);
        await context.SaveChangesAsync();
    }
    if (!await context.Users.AnyAsync(u => u.Role == OptionsRole.USER))
    {
        var senhaHash_client = await hasher.HashPassword("amora0909!");
        var DataUser_client = new User
        {
            Nome = "Cliente_MidasAPI",
            Email = "Cliente@gmail.com",
            CPF = "15385638402",
            PasswordHash = senhaHash_client,
            Role = OptionsRole.USER,
            DataNascimento = new DateTime(2008, 01, 30),
            Status = OptionsStatus.ATIVO,
            CreatedAt = DateTime.Now,
            Telefone = "31987653872"
        };
        var DataAccount_client = new Accounts
        {
            ChavePix = "11111111111",
            User = DataUser_client,
            NumeroConta = "00000002",
            NumeroAgencia = "0001",
            Saldo = 100,
            TipoConta = OptionsTipoDaConta.CONTA_SALARIO,
            Status = OptionsStatus.ATIVO,
            CriadoEm = DateTime.Now,
        };
        context.Users.Add(DataUser_client);
        context.Accounts.Add(DataAccount_client);
        await context.SaveChangesAsync();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string? FindEnvFile()
{
    var currentDirectory = Directory.GetCurrentDirectory();

    while (true)
    {
        var candidate = Path.Combine(currentDirectory, ".env");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        var parent = Directory.GetParent(currentDirectory);
        if (parent is null)
        {
            return null;
        }

        currentDirectory = parent.FullName;
    }
}
