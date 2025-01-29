using Workly.Application.Interfaces;
using Workly.Application.Interfaces.Services;
using Workly.Application.Models;
using Workly.Application.Services;
using Workly.Domain.Interfaces;
using Workly.Infrastructure.Persistence.Repositories;
using Workly.Infrastructure.Services;

namespace Workly.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Generic
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Auth Services
            services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            //UserContext, userId aktarımı için
            services.AddScoped<IUserContext, UserContext>();

            //Mail
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IMailService, MailService>();

            

            return services;
        }
    }
}
