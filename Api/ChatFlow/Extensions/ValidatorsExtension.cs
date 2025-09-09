using ChatFlow.Validators;
using FluentValidation;

namespace ChatFlow.Extensions
{
    public static class ValidatorsExtension
    {
        public static IServiceCollection AddCustomValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<LoginValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

            return services;
        }
    }
}
