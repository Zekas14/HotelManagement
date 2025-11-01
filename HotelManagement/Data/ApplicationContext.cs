using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Facility> Facilities { get; set; }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IConfiguration configuration ;
        public ApplicationDbContextFactory()
        {
            this.configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
             configuration.GetConnectionString("DefaultConnection"),
             options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
             ).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            return new ApplicationDbContext(optionsBuilder.Options);
        }

    }
}
