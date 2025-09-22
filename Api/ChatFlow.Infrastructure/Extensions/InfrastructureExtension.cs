using ChatFlow.Domain.Interfaces;
using ChatFlow.Infrastructure.Repositories;
using ChatFlow.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatFlow.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


            // Other services
            services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IRefreshTokenProvider, RefreshTokenProvider>();

            return services;
        }
    }
    //EF Core registra ApplicationDbContext como Scoped; por consistencia, los repos también en Scoped.
}
