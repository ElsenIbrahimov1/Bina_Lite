using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PropertyMediaConfiguration : IEntityTypeConfiguration<PropertyMedia>
{
    public void Configure(EntityTypeBuilder<PropertyMedia> builder)
    {
        builder.ToTable("PropertyMedia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MediaUrl)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.MediaType)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Order)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        // Relationship
        builder.HasOne(x => x.PropertyAd)
               .WithMany(x => x.Media)
               .HasForeignKey(x => x.PropertyAdId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.PropertyAdId);

        builder.HasIndex(x => new
        {
            x.PropertyAdId,
            x.Order
        })
        .IsUnique();
    }
}
