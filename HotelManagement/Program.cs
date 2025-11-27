using FastEndpoints;
using Hangfire;
using Hangfire.PostgreSql;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement;
using HotelManagement.Features.RoomManagement;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Infrastructure.Data.Seeds;
using HotelManagement.Infrastructure.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
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
builder.Services.AddRateLimiter();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>()); 
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
    options.UseExistingNpgsqlConnection(new NpgsqlConnection(builder.Configuration.GetConnectionString("PostConnection"))))
);
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DatabaseSeeder.Seed(context);
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.UseHangfireDashboard("/hangfire");
app.UseFastEndpoints().UseRateLimiter(new RateLimiterOptions
{
    OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var response = new FailureEndpointResult<bool>(errorCode: ErrorCode.LimitReached, message: "Slow down ya man… too many requests!");
        await context.HttpContext.Response.WriteAsJsonAsync(response, ct);
    }
} );
app.Run();

