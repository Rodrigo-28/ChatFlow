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
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IConversationRepository, ConversationRepository>();

            // Other services
            services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}
