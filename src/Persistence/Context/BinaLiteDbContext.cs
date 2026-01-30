using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence.Context;

public class BinaLiteDbContext: DbContext
{
    public BinaLiteDbContext(DbContextOptions<BinaLiteDbContext> options) : base(options)
    {
    }


    public DbSet<PropertyAd> PropertyAds { get; set; }

    public DbSet<PropertyMedia> PropertyMedias { get; set; }

    public DbSet<City> Cities { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        modelBuilder.ApplyConfiguration(new PropertyAdConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyMediaConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());



        base.OnModelCreating(modelBuilder);
    }


}
