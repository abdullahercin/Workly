using Microsoft.AspNetCore.Mvc;
using Workly.API.Models;
using Workly.Application.DTOs.Users;
using Workly.Application.Interfaces;

namespace Workly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto,
            CancellationToken cancellationToken)
        {
            // Kullanıcıyı oluştur
            var userId = await userService.CreateUserAsync(registerUserDto, cancellationToken);

            // Oluşan kullanıcı Id'sini dön
            return StatusCode(StatusCodes.Status201Created, ApiResponse<int>.Success(userId));
        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken cancellationToken)
        {
            try
            {
                var result = await userService.ConfirmEmailAsync(token, cancellationToken);
                if(result) return Ok(ApiResponse<string>.Success("Email başarıyla doğrulandı."));
                return BadRequest(ApiResponse<string>.Fail("Email doğrulama başarısız."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail("Email doğrulama başarısız: " + ex.Message));
            }
        }
    }
}
