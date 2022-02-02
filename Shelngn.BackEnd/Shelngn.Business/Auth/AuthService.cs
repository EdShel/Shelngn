using Shelngn.Exceptions;
using Shelngn.Models;
using Shelngn.Repositories;
using Shelngn.Services;
using Shelngn.Services.Auth;
using System.Security.Claims;

namespace Shelngn.Business.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserStore appUserStore;

        private readonly IJwtTokenGenerator jwtTokenGenerator;

        private readonly IRefreshTokenFactory refreshTokenFactory;

        private readonly IPasswordHasher passwordHasher;

        private readonly IUnitOfWork unitOfWork;

        private readonly IRepositoryFactory repositoryFactory;

        public AuthService(
            IAppUserStore appUserStore,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenFactory refreshTokenFactory,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IRepositoryFactory repositoryFactory)
        {
            this.appUserStore = appUserStore;
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.refreshTokenFactory = refreshTokenFactory;
            this.passwordHasher = passwordHasher;
            this.unitOfWork = unitOfWork;
            this.repositoryFactory = repositoryFactory;
        }

        public async Task RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            AppUser? anotherUserWithEmail = await this.appUserStore.FindByEmailAsync(request.Email, ct);
            if (anotherUserWithEmail != null)
            {
                throw new ConflictException("User with email already exists.");
            }
            if (request.UserName != null)
            {
                AppUser? anotherUserWithName = await this.appUserStore.FindByNameAsync(request.UserName, ct);
                if (anotherUserWithName != null)
                {
                    throw new ConflictException("User with name already exists.");
                }
            }

            ct.ThrowIfCancellationRequested();
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName!,
                PasswordHash = this.passwordHasher.HashPassword(request.Password)
            };

            await this.appUserStore.CreateAsync(user, ct);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest loginRequest, CancellationToken ct = default)
        {
            AppUser? user = await this.appUserStore.FindByEmailAsync(loginRequest.Email, ct);
            if (user == null)
            {
                throw new BadRequestException("User with email is not registered.");
            }

            if (!this.passwordHasher.VerifyPassword(user.PasswordHash, loginRequest.Password))
            {
                throw new BadRequestException("Password does not match.");
            }

            ct.ThrowIfCancellationRequested();

            RefreshToken newRefreshToken = this.refreshTokenFactory.GenerateRefreshToken(user.Id);
            var refreshTokenRepository = this.repositoryFactory.Create<IRefreshTokenRepository>(this.unitOfWork);
            await refreshTokenRepository.CreateAsync(newRefreshToken, ct);

            ClaimsIdentity tokenClaims = GetTokenClaimsForUser(user);
            return new LoginResult(
                AccessToken: this.jwtTokenGenerator.GenerateAuthToken(tokenClaims),
                RefreshToken: newRefreshToken.Value
            );
        }

        private ClaimsIdentity GetTokenClaimsForUser(AppUser user)
        {
            var userClaims = new[]
            {
                new Claim(AuthConstants.Claims.ID, user.Id.ToString()),
            };
            return new ClaimsIdentity(userClaims);
        }

        public async Task<RefreshResult> RefreshTokenAsync(RefreshRequest refreshRequest, CancellationToken ct = default)
        {
            string authHeaderValue = refreshRequest.AuthHeaderValue;
            var userPrincipal = this.jwtTokenGenerator.ExtractPrincipalFromExpiredAuthHeader(authHeaderValue);
            if (userPrincipal == null)
            {
                throw new BadRequestException("Access token is invalid.");
            }

            string id = userPrincipal.FindFirstValue(AuthConstants.Claims.ID);
            AppUser? user = await this.appUserStore.FindByIdAsync(id);
            if (user == null)
            {
                throw new BadRequestException("User does not exist.");
            }

            IRefreshTokenRepository refreshTokenRepository = this.repositoryFactory.Create<IRefreshTokenRepository>(this.unitOfWork);

            RefreshToken? refreshToken = await refreshTokenRepository.GetByValueAsync(refreshRequest.RefreshToken, ct);
            if (refreshToken == null)
            {
                throw new BadRequestException("Refresh token does not exist.");
            }

            if (refreshToken.Expires < DateTimeOffset.UtcNow)
            {
                throw new BadRequestException("Refresh token has expired.");
            }

            ct.ThrowIfCancellationRequested();

            this.unitOfWork.Begin();
            try
            {
                await refreshTokenRepository.DeleteAsync(refreshRequest.RefreshToken, ct);
                var newRefreshToken = this.refreshTokenFactory.GenerateRefreshToken(user.Id);
                await refreshTokenRepository.CreateAsync(newRefreshToken, ct);
                this.unitOfWork.Commit();

                return new RefreshResult(
                    AccessToken: this.jwtTokenGenerator.GenerateAuthToken(userPrincipal.Identities.First()),
                    RefreshToken: newRefreshToken.Value
                );
            }
            catch (Exception)
            {
                this.unitOfWork.Rollback();
                throw;
            }
        }

        public async Task RevokeTokenAsync(RevokeRequest revokeRequest, CancellationToken ct = default)
        {
            AppUser? user = await this.appUserStore.FindByIdAsync(revokeRequest.UserId, ct);
            if (user == null)
            {
                throw new BadRequestException("User does not exist.");
            }

            IRefreshTokenRepository refreshTokenRepository = this.repositoryFactory.Create<IRefreshTokenRepository>(this.unitOfWork);
            RefreshToken? refreshToken = await refreshTokenRepository.GetByValueAsync(revokeRequest.RefreshToken);
            if (refreshToken == null)
            {
                throw new NotFoundException("Refresh token does not exist.");
            }

            await refreshTokenRepository.DeleteAsync(refreshToken.Value, ct);
        }
    }
}
