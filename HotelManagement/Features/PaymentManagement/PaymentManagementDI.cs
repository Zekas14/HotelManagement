using FluentValidation;
using HotelManagement.Features.PaymentManagement.ProcessPayment;

namespace HotelManagement.Features.PaymentManagement
{
    public static class PaymentManagementDI
    {
        public static IServiceCollection AddPaymentManagementFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<ProcessPaymentCommand>, ProcessPaymentValidator>();
            return services;
        }
    }
}
