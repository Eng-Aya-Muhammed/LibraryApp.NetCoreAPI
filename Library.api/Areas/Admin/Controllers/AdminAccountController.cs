using Library.core;
using Library.core.DTO;
using Library.core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.api.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Librarian")]
    public class AdminAccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthRepository authService;

        public AdminAccountController(IUnitOfWork unitOfWork, IAuthRepository authService)
        {
            _unitOfWork = unitOfWork;
            this.authService = authService;
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.RegisterAdminAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);


            return Ok(result);
        }
        [HttpPost("AcceptUser/{id}")]
        public async Task<IActionResult> AcceptUser(string id)
        {
            var user = authService.AcceptUser(id);
            if (user == null)
                return NotFound(); // or return appropriate status code

            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }
        [HttpPost("RejectUser/{id}")]
        public async Task<IActionResult> RejectUser(string id)
        {
            var user = authService.RejectUser(id);
            if (user == null)
                return NotFound("This id not found !");

            await _unitOfWork.CompleteAsync();

            return Ok(user);
        }
        
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        [HttpGet("GetUser/{Email}")]
        public IActionResult GetUser(string Email)
        {
            var user = authService.GetUser(Email);
            return Ok(user);
        }
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users =authService.GetUsers();
            return Ok(users);
        }
    }
}
