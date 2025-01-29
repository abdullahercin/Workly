using System.Security.Claims;
using Workly.Domain.Entities;
using Workly.Domain.Models;

namespace Workly.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> roles);
        RefreshToken CreateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
} 