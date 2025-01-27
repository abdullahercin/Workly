
namespace Workly.Application.Interfaces
{
    public interface IAuthService
    {
        string? Authenticate(string username, string password, string role);
    }
}
