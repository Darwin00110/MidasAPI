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
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IHashService>();

    if (!context.Users.Any(u => u.Role == OptionsRole.ADMIN))
    {
        var senhaHash = await hasher.HashPassword("amora0909!!!!");
        context.Users.Add(new User
        {
            Nome = "Isaque",
            Email = "Admin@gmail.com",
            PasswordHash = senhaHash,
            Role = OptionsRole.ADMIN
        });
        await context.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
