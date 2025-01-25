
namespace Workly.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        string? Authenticate(string username, string password, string role);
    }
}
