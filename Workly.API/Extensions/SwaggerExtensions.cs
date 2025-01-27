using Microsoft.OpenApi.Models;
using Workly.API.Models;

namespace Workly.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workly API", Version = "v1" });

                // ApiResponse tipi için schema ekleme
                c.MapType(typeof(ApiResponse<>), () => new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        { "issuccess", new OpenApiSchema { Type = "boolean" } },
                        { "message", new OpenApiSchema { Type = "string" } },
                        { "data", new OpenApiSchema { Type = "object" } }, // Generic veri için daha detaylı yapılandırma gerekebilir
                        { "errors", new OpenApiSchema { Type = "list<string>" } },
                    }
                });
            });
            
            return services;
        }

        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workly API v1");
            });

            return app;
        }
    }
}
