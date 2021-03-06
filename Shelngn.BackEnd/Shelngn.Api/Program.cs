using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Shelngn.Api.Filters;
using Shelngn.Api.Utilities;
using Shelngn.Api.Workspaces;
using Shelngn.Business;
using Shelngn.Business.Auth;
using Shelngn.Business.FileUpload;
using Shelngn.Business.GameProjects.Build;
using Shelngn.Business.GameProjects.Crud;
using Shelngn.Business.GameProjects.Files;
using Shelngn.Business.GameProjects.Publishing;
using Shelngn.Business.Workspaces;
using Shelngn.Business.Workspaces.ActiveUsers;
using Shelngn.Business.Workspaces.ProjectFiles;
using Shelngn.Data;
using Shelngn.Data.Repositories;
using Shelngn.Repositories;
using Shelngn.Services;
using Shelngn.Services.Auth;
using Shelngn.Services.FileUpload;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Build;
using Shelngn.Services.GameProjects.Crud;
using Shelngn.Services.GameProjects.Files;
using Shelngn.Services.GameProjects.Publishing;
using Shelngn.Services.Workspaces;
using Shelngn.Services.Workspaces.ActiveUsers;
using Shelngn.Services.Workspaces.ProjectFiles;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Configurations
services.Configure<ConnectionStringProvider>(builder.Configuration.GetSection("ConnectionStrings"));
services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationSettings"));
AuthenticationSettings jwtSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
services.Configure<GameProjectCreateSettings>(configuration.GetSection("GameProjectCreationSettings"));
string[] corsAllowedOrigins = configuration.GetValue<string>("CorsAllowedOrigins")?.Split(";")
    ?? throw new InvalidOperationException("CORS configuration 'CorsAllowedOrigins' is not given.");

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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => // for SignalR clients authentication
            {
                string accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/workspace"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    }
);
services.AddAuthorization(options =>
{
    options.AddPolicy(GameProjectAuthPolicy.JustBeingMember, b => b.AddRequirements(new GameProjectMemberRequirement(new GameProjectRights())));
    options.AddPolicy(GameProjectAuthPolicy.ChangeMembers, b => b.AddRequirements(new GameProjectMemberRequirement(new GameProjectRights { ChangeMembers = true })));
    options.AddPolicy(GameProjectAuthPolicy.WorkspaceWrite, b => b.AddRequirements(new GameProjectMemberRequirement(new GameProjectRights { Workspace = true })));
    options.AddPolicy(GameProjectAuthPolicy.Publishing, b => b.AddRequirements(new GameProjectMemberRequirement(new GameProjectRights { Publishing = true })));
});
services.AddTransient<IAuthorizationHandler, GameProjectMemberAuthorizationHandler>();

services.AddControllers();
services.AddHttpContextAccessor();

services.AddSignalR();
services.AddSingleton<IUserIdProvider, IdClaimUserIdProvider>();

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
services.AddTransient<IGameProjectPublicationRepository, GameProjectPublicationRepository>();
services.AddTransient<IGameProjectScreenshotRepository, GameProjectScreenshotRepository>();

// Services
services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
services.AddTransient<IRefreshTokenFactory, RefreshTokenFactory>();
services.AddTransient<IAuthService, AuthService>();
services.AddTransient<IAppUserStore, AppUserStore>();
services.AddSingleton<IFileSystem, LocalFileSystem>();
services.AddTransient<IGameProjectCreator, GameProjectCreator>();
services.AddTransient<IGameProjectUpdater, GameProjectUpdater>();
services.AddTransient<IGameProjectSearcher, GameProjectSearcher>();
services.AddSingleton<IGameProjectStorageBalancer, GameProjectStorageBalancer>(p => new GameProjectStorageBalancer(configuration.GetValue<string>("ProjectsDirectory")));
services.AddSingleton<IGameProjectBuilder, IGameProjectBuilder>(p => new GameProjectBuilder(configuration.GetValue<string>("ProjectsDirectory"), p.GetRequiredService<ILogger<GameProjectBuilder>>()));
services.AddSingleton<IGameProjectBuildResultAccessor, GameProjectBuildResultAccessor>(p => new GameProjectBuildResultAccessor(configuration.GetValue<string>("ProjectsDirectory")));
services.AddSingleton<IProjectFileAccessor, ProjectFileAccessor>(p => new ProjectFileAccessor(configuration.GetValue<string>("ProjectsDirectory")));
services.AddTransient<IGameProjectAuthorizer, GameProjectAuthorizer>();
services.AddSingleton<IFileUploadUrlSigning, FileUploadUrlSigning>(opt => new FileUploadUrlSigning(configuration.GetValue<string>("SigningPrivateKey")));
services.AddTransient<IGameProjectDeleter, GameProjectDeleter>();
services.AddTransient<IContentTypeProvider, FileExtensionContentTypeProvider>();
services.AddTransient<IGameProjectPublisher, GameProjectPublisher>();

services.AddSingleton<IWorkspacesStatesManager, WorkspacesStatesManager>();
services.AddTransient<IActiveUsersWorkspaceStateReducer, ActiveUsersWorkspaceStateReducer>();
services.AddTransient<IProjectFilesWorkspaceStateReducer, ProjectFilesWorkspaceStateReducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder
    .WithOrigins(corsAllowedOrigins)
    .AllowCredentials()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<WorkspaceHub>("/workspace");

app.Run();
