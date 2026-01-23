using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context;

public class BinaLiteDbContext: DbContext
{
    public BinaLiteDbContext(DbContextOptions<BinaLiteDbContext> options) : base(options)
    {
    }

    public DbSet<PropertyAd> PropertyAds { get; set; }

    public DbSet<PropertyMedia> PropertyMedias { get; set; }


}
