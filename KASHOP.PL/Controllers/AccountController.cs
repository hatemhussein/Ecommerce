using KASHOP.BL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);

            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var isConfirmed = await _authenticationService.ConfirmEmailAsync(token, userId);
            if (isConfirmed) return Ok();
            return BadRequest();
        }

        [HttpPost("SendCode")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset(ForgotPasswordRequest request)
        {
            var result = await _authenticationService.RequestResetPasswordAsync(request);

            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _authenticationService.ResetPasswordAsync(request);

            if(!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var result = await _authenticationService.RefreshTokenAsync();
            if(!result.Success) return Unauthorized(result);
            return Ok(result);
        }

    }
}
