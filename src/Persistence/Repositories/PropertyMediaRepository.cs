using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PropertyMediaRepository : GenericRepository<PropertyMedia, int>, IPropertyMediaRepository
{
    public PropertyMediaRepository(BinaLiteDbContext context) : base(context) { }

    public Task<List<PropertyMedia>> GetByPropertyAdIdAsync(int propertyAdId, CancellationToken ct = default)
        => _table.AsNoTracking()
            .Where(x => x.PropertyAdId == propertyAdId)
            .OrderBy(x => x.Order)
            .ToListAsync(ct);
}