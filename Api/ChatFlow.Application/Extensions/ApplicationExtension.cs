using ChatFlow.Application.Interfaces;
using ChatFlow.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatFlow.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IConversationService, ConversationService>();

            services.AddSingleton<WebSocketHandler>();

            return services;
        }
    }
}
