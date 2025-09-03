using Microsoft.AspNetCore.Mvc;
using SlaytifiedApi.Application.Dtos;
using SlaytifiedApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace SlaytifiedApi.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }


        // Protect this endpoint(what if i want to use my own middleware?)
        // Todo: What if i'm to use my own middleware.
        // [AllowAnonymous]
        [Authorize]
        [HttpGet("getUser")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine($"ClaimTypes oooooooooo: {ClaimTypes.NameIdentifier}");
            Console.WriteLine($"User oooooooooo: {User}");
            Console.WriteLine($"userIdClaim: {userIdClaim}");

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var user = await _authService.GetUserByIdAsync(userId);

            return Ok(user);
        }
    }
}
