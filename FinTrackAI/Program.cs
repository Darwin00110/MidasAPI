using FinTrackAI;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using FinTrackAI.src.Application.UseCase;
using FinTrackAI.src.Domain.Interfaces.UserUseCase_NoService;
using FinTrackAI.src.Domain.Interfaces.IUserUseCase;
using FinTrackAI.src.Infra.Services.Hash;
using FinTrackAI.src.Domain.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
));

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
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

    app.UseSwagger();
    app.UseSwaggerUI();
    Open_Swagger.Open();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IHashService>();

    if (!context.Users.Any(u => u.Role == OptionsRole.ADMIN))
    {
        var senhaHash = await hasher.HashPassword("amora0909!");
        var DataUser = new User
        {
            Nome = "Banco_FinTrackAI",
            Email = "Admin@gmail.com",
            CPF = "15385638406",
            PasswordHash = senhaHash,
            Role = OptionsRole.ADMIN,
            DataNascimento = new DateTime(2008, 01, 30),
            Status = OptionsStatus.ATIVO,
            CreatedAt = DateTime.Now,
            Telefone = "31-98765-3872"
        };
        var DataAccount = new Accounts
        {
          ChavePix = "00000000000",
          User = DataUser,
          Saldo = 1000000,
          TipoConta = OptionsTipoDaConta.CONTA_SALARIO,
          Status = OptionsStatus.ATIVO,
          CriadoEm = DateTime.Now,
        };
        context.Users.Add(DataUser);
        context.Accounts.Add(DataAccount);
        await context.SaveChangesAsync();
    }
    if(!context.Users.Any(u => u.Role == OptionsRole.USER)){
        var senhaHash_client = await hasher.HashPassword("amora0909!");
        var DataUser_client = new User
        {
            Nome = "Cliente_FinTrackAI",
            Email = "Cliente@gmail.com",
            CPF = "15385638402",
            PasswordHash = senhaHash_client,
            Role = OptionsRole.USER,
            DataNascimento = new DateTime(2008, 01, 30),
            Status = OptionsStatus.ATIVO,
            CreatedAt = DateTime.Now,
            Telefone = "31-98765-3872"
        };
        var DataAccount_client = new Accounts
        {
          ChavePix = "11111111111",
          User = DataUser_client,
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
