using FluentValidation;
using HotelManagement.Features.AuthManagement.Login;
using HotelManagement.Features.AuthManagement.Register;

namespace HotelManagement.Features.AuthManagement
{
    public static class AuthManagementDI
    {
        public static IServiceCollection AddAuthManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IValidator<RegisterCommand>, RegisterValidator>();
            services.AddScoped<IValidator<LoginCommand>, LoginValidator>();
            return services;
        }
    }
}
