using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PropertyAdConfiguration : IEntityTypeConfiguration<PropertyAd>
{
    public void Configure(EntityTypeBuilder<PropertyAd> builder)
    {
        builder.ToTable("PropertyAds");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.AreaInSquareMeters)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.RoomCount)
               .IsRequired();

        builder.Property(x => x.IsMortgage)
               .IsRequired();

        builder.Property(x => x.IsExtract)
               .IsRequired();

        builder.Property(x => x.OfferType)
               .IsRequired();

        builder.Property(x => x.PropertyCategory)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        // Relationships
        builder.HasMany(x => x.Media)
               .WithOne(x => x.PropertyAd)
               .HasForeignKey(x => x.PropertyAdId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.Price);
        builder.HasIndex(x => x.RoomCount);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.OfferType);
        builder.HasIndex(x => x.PropertyCategory);

        builder.HasIndex(x => new
        {
            x.OfferType,
            x.PropertyCategory
        });

        builder.HasIndex(x => new
        {
            x.OfferType,
            x.PropertyCategory,
            x.Price
        });
    }
}
