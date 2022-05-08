using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services;
using Shelngn.Services.Auth;

namespace Shelngn.Api.Auth
{
    [ApiController]
    [Route("[controller]")]
    public partial class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IAppUserStore appUserStore;
        private readonly IMapper mapper;

        public AuthController(IAuthService authService, IAppUserStore appUserStore, IMapper mapper)
        {
            this.authService = authService;
            this.appUserStore = appUserStore;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterModel request, CancellationToken ct)
        {
            var registerDTO = new RegisterRequest
            (
                Email: request.Email,
                UserName: request.UserName,
                Password: request.Password
            );
            await this.authService.RegisterAsync(registerDTO, ct);

            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken ct)
        {
            var loginDTO = new LoginRequest
            (
                Email: request.Email,
                Password: request.Password
            );
            LoginResult response = await this.authService.LoginAsync(loginDTO, ct);

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshTokenAsync([FromBody] RefreshModel request, CancellationToken ct)
        {
            bool hasAuthHeader = this.Request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues tokens);
            if (!hasAuthHeader || tokens.Count > 1)
            {
                throw new BadRequestException("Authorization header is missing or has 2+ values.");
            }

            var requestDTO = new RefreshRequest
            (
                AuthHeaderValue: tokens.First(),
                RefreshToken: request.RefreshToken
            );

            RefreshResult response = await this.authService.RefreshTokenAsync(requestDTO, ct);

            return Ok(response);
        }

        [HttpPost("revoke"), Authorize]
        public async Task<ActionResult> RevokeTokenAsync([FromBody] RevokeModel request, CancellationToken ct)
        {
            var requestDTO = new RevokeRequest(
                UserId: this.User.GetIdGuid(),
                RefreshToken: request.RefreshToken
            );
            await this.authService.RevokeTokenAsync(requestDTO, ct);

            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            Guid currentUserId = this.User.GetIdGuid();
            AppUser user = await this.appUserStore.FindByIdAsync(currentUserId, ct)
                ?? throw new InvalidOperationException("User who owns access token does not exist.");
            CurrentUserModel currentUser = mapper.Map<CurrentUserModel>(user);
            return Ok(currentUser);
        }
    }
}
