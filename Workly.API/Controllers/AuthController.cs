using Microsoft.AspNetCore.Mvc;
using Workly.Application.DTOs.Auth;
using Workly.Application.Interfaces;

namespace Workly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = authService.Authenticate(request.Email, request.Password, request.Role);

            if (token != null)
            {
                return Ok(token);
            }

            return Unauthorized();
        }
    }
}
