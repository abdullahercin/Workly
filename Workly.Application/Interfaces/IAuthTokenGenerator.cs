namespace Workly.Application.Interfaces
{
    public interface IAuthTokenGenerator
    {
        string? GenerateJwtToken(string username, string role);
    }
}
