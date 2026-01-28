using Application.Abstracts.Repositories;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : notnull
{
    protected readonly BinaLiteDbContext _context;
    protected readonly DbSet<TEntity> _table;

    public GenericRepository(BinaLiteDbContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query()
    {
        return _table.AsQueryable();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
     
        return await _table.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _table.ToListAsync(ct);
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _table.AddAsync(entity, ct);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await _table.AddRangeAsync(entities, ct);
    }

    public void Update(TEntity entity)
    {
        _table.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _table.Remove(entity);
    }

    public async Task DeleteByIdAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null)
            return;

        _table.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
