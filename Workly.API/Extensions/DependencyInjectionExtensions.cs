using Workly.Application.Interfaces;
using Workly.Application.Services;
using Workly.Domain.Interfaces;
using Workly.Infrastructure.Persistence.Repositories;
using Workly.Infrastructure.Services;

namespace Workly.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Generic
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Auth Services
            services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();

            //UserContext, userId aktarımı için
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}
