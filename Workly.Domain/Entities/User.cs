using Microsoft.AspNetCore.Identity;

namespace Workly.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = false;
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
