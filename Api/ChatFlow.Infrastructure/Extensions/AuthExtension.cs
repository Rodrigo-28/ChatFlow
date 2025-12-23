// ChatFlow/Extensions/AuthExtensions.cs
using ChatFlow.Infrastructure.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatFlow.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
                        ),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(5) // pequeña tolerancia
                    };

                    // Validación extra: token versioning ("ver")
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

                            // Buscar 'sub' tanto por claim estándar como por el mapeo a NameIdentifier
                            var sub =
                                context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var ver = context.Principal?.FindFirst("ver")?.Value;

                            if (!Guid.TryParse(sub, out var userId))
                            {
                                context.Fail("Missing or invalid 'sub' claim.");
                                return;
                            }

                            if (!int.TryParse(ver, out var tokenVersionInClaim))
                            {
                                context.Fail("Missing or invalid 'ver' claim.");
                                return;
                            }

                            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                            if (user is null)
                            {
                                context.Fail("User not found in DB.");
                                return;
                            }

                            if (user.TokenVersion != tokenVersionInClaim)
                            {
                                context.Fail($"Token version mismatch. DB={user.TokenVersion}, Claim={tokenVersionInClaim}");
                                return;
                            }
                        },

                        OnAuthenticationFailed = context =>
                        {
                            // Log útil en desarrollo; en prod podés bajarlo a Debug/Trace o quitarlo.
                            Console.WriteLine($"Auth failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            // Añade detalle en Dev para ver causa en el header WWW-Authenticate
                            var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
                            if (!context.Handled && env.IsDevelopment())
                            {
                                var desc = context.ErrorDescription ?? context.AuthenticateFailure?.Message ?? "invalid token";
                                context.Response.Headers["WWW-Authenticate"] =
                                    $"{context.Options.Challenge} error=\"invalid_token\", error_description=\"{desc}\"";
                            }
                            return Task.CompletedTask;
                        }
                    };

                });

            services.AddAuthorization();
            return services;
        }
    }
}
