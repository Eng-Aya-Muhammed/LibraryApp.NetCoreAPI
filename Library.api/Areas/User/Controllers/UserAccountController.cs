using Library.core;
using Library.core.DTO;
using Library.core.Interfaces;
using Library.EF.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library.api.Areas.User.Controllers
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthRepository authService;

        public UserAccountController(IUnitOfWork unitOfWork, IAuthRepository authService)
        {
            _unitOfWork = unitOfWork;
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(result);

            return Ok("Registration successful. Please wait for approval from the admin.");
        }


        
        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] TokenRequestDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(result.Message);
            
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                

            }
            var user = authService.GetUser(model.Email);

            if (user != null && user.IsApproved)
            {
                    return Ok(result);

            }
            else
                return Unauthorized("User not approved by admin.");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResult = await authService.Logout();
            if (logoutResult)
            {
                return Ok(new { message = "Logout successful" });
            }
            else
            {
                return BadRequest(new { message = "Logout failed" });
            }
        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result);
            }
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
                
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}