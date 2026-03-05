using HotelManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagement.Infrastructure.Configuration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasOne(i => i.Payment)
                   .WithOne(p => p.Invoice)
                   .HasForeignKey<Invoice>(i => i.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(100);
            builder.HasIndex(i => i.InvoiceNumber).IsUnique();
            builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
        }
    }
}
