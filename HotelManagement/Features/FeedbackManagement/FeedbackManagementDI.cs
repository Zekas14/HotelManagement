using FluentValidation;
using HotelManagement.Features.FeedbackManagement.SubmitReview;
using HotelManagement.Features.FeedbackManagement.RespondToReview;

// Feedback Management DI
namespace HotelManagement.Features.FeedbackManagement
{
    public static class FeedbackManagementDI
    {
        public static IServiceCollection AddFeedbackFeatures(this IServiceCollection services)
        {
            services.AddScoped<IValidator<SubmitReviewCommand>, SubmitReviewValidator>();
            services.AddScoped<IValidator<RespondToReviewCommand>, RespondToReviewValidator>();
            return services;
        }
    }
}
