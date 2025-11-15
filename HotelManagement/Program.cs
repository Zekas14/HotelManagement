using FastEndpoints;
using HotelManagement.Features.ReservationManagement;
using HotelManagement.Features.RoomManagement;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Infrastructure.Data.Seeds;
using HotelManagement.Infrastructure.Middlewares;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
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
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.UseFastEndpoints();
app.Run();

