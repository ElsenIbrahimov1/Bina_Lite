using Domain.Entities.Common;
namespace Application.Abstracts.Repositories;

public interface IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : notnull
{
    IQueryable<TEntity> Query();

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteByIdAsync(TKey id, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}

