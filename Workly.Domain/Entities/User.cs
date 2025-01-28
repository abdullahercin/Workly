namespace Workly.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = null!; // Kullanıcı email adresi
        public string PasswordHash { get; set; } = null!; // Şifre hashlenmiş hali
        public string Name { get; set; } = null!; // Kullanıcının adı
        public string? PhoneNumber { get; set; } // Telefon numarası (opsiyonel)
        public bool IsEmailConfirmed { get; set; } = false; // Email doğrulama durumu
        public string? EmailConfirmationToken { get; set; } // Email doğrulama için token
        public DateTime? EmailConfirmationTokenExpiry { get; set; } // Tokenin geçerlilik süresi
        public bool IsActive { get; set; } = false; // Kullanıcının aktif olup olmadığı durumu
        public bool IsLocked { get; set; } = false; // Kullanıcının kilitli olup olmadığı durumu
        public DateTime? LastLoginAt { get; set; } // Son giriş tarihi
    }
}
