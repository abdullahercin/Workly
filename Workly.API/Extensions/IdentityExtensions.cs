using Microsoft.AspNetCore.Identity;
using Workly.Domain.Entities;
using Workly.Infrastructure.Persistence.Context;

namespace Workly.API.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<int>>(options => 
            {
                // Şifre politikası
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Kullanıcı politikası
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                // Kilitleme politikası
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<WorklyDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
} 