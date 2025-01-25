using Workly.Application.Interfaces.Auth;
using Workly.Application.Services.Auth;
using Workly.Infrastructure.Services.Auth;

namespace Workly.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Auth Services
            services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
