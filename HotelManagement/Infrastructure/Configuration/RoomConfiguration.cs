using HotelManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HotelManagement.Infrastructure.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.RoomNumber).IsRequired();
            builder.HasIndex(r => r.RoomNumber).IsUnique();
            builder.Property(r => r.PricePerNight).HasColumnType("numeric(18,2)");
        }
    }
}
