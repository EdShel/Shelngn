using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shelngn.Api.Filters;
using Shelngn.Business;
using Shelngn.Business.Auth;
using Shelngn.Business.GameProjects;
using Shelngn.Data.Repositories;
using Shelngn.Repositories;
using Shelngn.Services;
using Shelngn.Services.Auth;
using Shelngn.Services.GameProjects;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Configurations
services.Configure<ConnectionStringProvider>(builder.Configuration.GetSection("ConnectionStrings"));
services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationSettings"));
AuthenticationSettings jwtSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();

TokenValidationParameters? jwtValidationOptions = new TokenValidationParameters
{
    ValidIssuer = jwtSettings.ValidIssuer,
    ValidateLifetime = jwtSettings.ValidateLifeTime,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
    ValidateIssuerSigningKey = true,
    ValidateIssuer = true,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero,
};
services.AddSingleton(jwtValidationOptions);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
    configureOptions: options =>
    {
        options.TokenValidationParameters = jwtValidationOptions;
        options.MapInboundClaims = false;
    }
);

services.AddControllers(options =>
{
    options.Filters.Add<ExceptionCatchingFilter>();
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAutoMapper(typeof(Program).Assembly);

// Repositories
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>();
services.AddTransient<IUnitOfWork, UnitOfWork>();
services.AddTransient<IRepositoryFactory, RepositoryFactory>();
services.AddTransient<IAppUserRepository, AppUserRepository>();
services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
services.AddTransient<IGameProjectRepository, GameProjectRepository>();

// Services
services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
services.AddTransient<IRefreshTokenFactory, RefreshTokenFactory>();
services.AddTransient<IAuthService, AuthService>();
services.AddTransient<IAppUserStore, AppUserStore>();
services.AddSingleton<LocalFileSystem>();
services.AddTransient<IGameProjectCreator, GameProjectCreator>();
services.AddTransient<IGameProjectSearcher, GameProjectSearcher>();
services.AddSingleton<IGameProjectStorageBalancer, GameProjectStorageBalancer>(p => new GameProjectStorageBalancer(@"C:\Users\Admin\Desktop\Projects\"));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
