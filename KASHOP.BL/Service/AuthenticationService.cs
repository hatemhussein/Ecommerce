using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Util;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return new RegisterResponse
                {
                    Message = "Failed",
                    Success = false,
                    Errors = result.Errors.Select(p => p.Description).ToList()
                };

            // giving a new user -> User role
            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);// because the website reads some of the special chars as %40% / %50% ..etc
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(user.Email, "Welcome", $"<h1> {request.UserName} </h1>" +
                $"" +
                $"<a href='{url}'>confirm </a>");

            return new RegisterResponse { Success = true, Message="Success" };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return new LoginResponse() { Success = false, Message = "Invalid Email" };

            if(! await _userManager.IsEmailConfirmedAsync(user))
                return new LoginResponse() { Success = false, Message = "Email is not confirmed" };

            if(await _userManager.IsLockedOutAsync(user))
                return new LoginResponse() { Success = false, Message = "account is blocked" };


            var passwprd = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwprd)
                return new LoginResponse() { Success = false, Message = "Invalid Password" };

            var refreshToken = await GenerateRefreshToken(user);
            SetRefreshTokenCookies(refreshToken);
            return new LoginResponse() { Success = true, Message = "Login Successful", AccessToken = await GenerateAccessToken(user) };
        }

        public async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(15);
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }
        public void SetRefreshTokenCookies(string refreshToken)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(15)
            });
        }
        public async Task<LoginResponse> RefreshTokenAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];

            if (refreshToken is null) {
                return new LoginResponse
                {
                    Success = false,
                    Message = "no refresh token"
                };
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Refresh token has expired"
                };
            }

            var newRefreshToken = await GenerateRefreshToken(user);
            SetRefreshTokenCookies(newRefreshToken);

            return new LoginResponse
            {
                Success = true,
                Message = "Success",
                AccessToken = await GenerateAccessToken(user)
        };
        }

        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync (userId);
            if (user is null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;
            return true;
        }

        public async Task<ForgotPasswordResponse> RequestResetPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user is null)
            {
                return new ForgotPasswordResponse()
                {
                    Message = "Invalid Email",
                    Success = false
                };
            }

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);
            await _emailSender.SendEmailAsync(request.Email, "Reset Password", $"<p>Code is {code}</p>");

            return new ForgotPasswordResponse()
            {
                Message = "Code has been sent to your email",
                Success = true
            };
        }
        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user is null)
            {
                return new ResetPasswordResponse()
                {
                    Message = "Invalid Email",
                    Success = false
                };
            }
            else if(user.CodeResetPassword != request.Code)
            {
                return new ResetPasswordResponse()
                {
                    Message = "Invalid code",
                    Success = false
                };
            }
            else if(user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse()
                {
                    Message = "Code Expired",
                    Success = false
                };
            }

            var isSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if(isSamePassword)
            {
                return new ResetPasswordResponse()
                {
                    Message = "New Password must be different than older ones",
                    Success = false
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user,token, request.NewPassword);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponse()
                {
                    Message = "Password Reset Failed",
                    Success = false
                };
            }
            return new ResetPasswordResponse()
            {
                Message = "Password has been reset",
                Success = true
            };
        }


    }
}
