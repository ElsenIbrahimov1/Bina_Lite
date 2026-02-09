using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence.Context;

public class BinaLiteDbContext : IdentityDbContext<AppUser>
{
    public BinaLiteDbContext(DbContextOptions<BinaLiteDbContext> options) : base(options) { }

    public DbSet<PropertyAd> PropertyAds { get; set; }
    public DbSet<PropertyMedia> PropertyMedias { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // IMPORTANT for Identity tables

        modelBuilder.ApplyConfiguration(new PropertyAdConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyMediaConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new DistrictConfiguration());
    }
}
