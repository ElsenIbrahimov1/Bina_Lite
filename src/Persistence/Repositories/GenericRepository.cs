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

    public TEntity GetById(TKey id)
    {
        return _table.Find(id);
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _table.ToList();
    }

    public void Add(TEntity entity)
    {
        _table.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _table.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        _table.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _table.Remove(entity);
    }

    public void DeleteById(TKey id)
    {
        var entity = _table.Find(id);
        if (entity is null)
            return;

        _table.Remove(entity);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
