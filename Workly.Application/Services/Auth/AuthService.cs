using Workly.Application.Interfaces.Auth;

namespace Workly.Application.Services.Auth
{
    public class AuthService(IAuthTokenGenerator tokenGenerator) : IAuthService
    {
        public string? Authenticate(string username, string password, string role)
        {
            // Kullanıcı doğrulama işlemleri burada yapılır
            if (role == "Admin")
            {
                if (username == "admin" && password == "password")  // Örnek doğrulama
                {
                    return tokenGenerator.GenerateJwtToken(username, "Admin");
                }
            }

            if (username == "personel" && password == "password")
            {
                return tokenGenerator.GenerateJwtToken(username, "Personel");
            }

            return null;
        }
    }
}
