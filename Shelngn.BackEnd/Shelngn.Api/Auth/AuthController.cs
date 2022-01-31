using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Shelngn.Exceptions;
using Shelngn.Services.Auth;

namespace Shelngn.Api.Auth
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
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

    }
}
