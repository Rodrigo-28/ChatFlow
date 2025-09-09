using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ChatFlow.Infrastructure.Extensions
{
    public static class SwaggerServiceExtension
    {

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookbase API", Version = "v1" });

                //Defining security Scheme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your token"
                });

                // Add the security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            return services;

        }
    }
}
