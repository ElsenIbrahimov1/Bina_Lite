using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PropertyMediaConfiguration : IEntityTypeConfiguration<PropertyMedia>
{
    public void Configure(EntityTypeBuilder<PropertyMedia> builder)
    {
        builder.ToTable("PropertyMedias");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ObjectKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasOne(x => x.PropertyAd)
            .WithMany(x => x.Media)
            .HasForeignKey(x => x.PropertyAdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.PropertyAdId);

        builder.HasIndex(x => new { x.PropertyAdId, x.Order })
            .IsUnique();
    }
}
