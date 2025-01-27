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
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcıyı oluştur
                var userId = await userService.CreateUserAsync(registerUserDto, cancellationToken);

                // Oluşan kullanıcı Id'sini dön
                return Ok(ApiResponse<int>.Success(userId,
                    "Kayıt başarılı, Lütfen e-postanızı kontrol edip doğrulama işlemini yapın."));
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun bir yanıt döndür
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
