using FastEndpoints;
using Hangfire;
using Hangfire.PostgreSql;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement;
using HotelManagement.Features.RoomManagement;
using HotelManagement.Features.PaymentManagement;
using HotelManagement.Features.AuthManagement;
using HotelManagement.Features.ReportingManagement;
using HotelManagement.Features.FeedbackManagement;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Infrastructure.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostConnection"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);
builder.Services.AddMemoryCache(); 
builder.Services.AddRoomManagementFeatures();
builder.Services.AddReservationManagementFeatures();
builder.Services.AddPaymentManagementFeatures();
builder.Services.AddAuthManagementFeatures();
builder.Services.AddReportingFeatures();
builder.Services.AddFeedbackFeatures();
builder.Services.AddRateLimiter();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>()); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = jwtSettings["Key"] ?? throw new ArgumentNullException("JWT Key is missing");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
    options.UseExistingNpgsqlConnection(new NpgsqlConnection(builder.Configuration.GetConnectionString("PostConnection"))))
);
builder.Services.AddCors(setupactions=>
{
    setupactions.AddPolicy("AllowOrigins",
        policy =>
        {
            policy.AllowAnyOrigin();
        });   
});
builder.Services.AddHangfireServer();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/* using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<ApplicationDbContext>();
//     DatabaseSeeder.Seed(context);
// }
*/
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseCors("AllowOrigins");
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.UseFastEndpoints().UseHangfireDashboard("/hangfire").UseRateLimiter(new RateLimiterOptions
{
    OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var response = new FailureEndpointResult<bool>(errorCode: ErrorCode.LimitReached, message: "Slow down ya man� too many requests!");
        await context.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }
} 
);
app.Run();

