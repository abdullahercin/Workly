namespace Workly.Application.Interfaces.Auth
{
    public interface IAuthTokenGenerator
    {
        string? GenerateJwtToken(string username, string role);
    }
}
