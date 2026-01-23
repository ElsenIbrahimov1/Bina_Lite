using Domain.Entities.Common;
namespace Application.Abstracts.Repositories;

public interface IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : notnull
{
    IQueryable<TEntity> Query();
    TEntity? GetById(TKey id);
    IEnumerable<TEntity> GetAll();

    
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    
    void Update(TEntity entity);

    
    void Delete(TEntity entity);
    void DeleteById(TKey id);

    
    void SaveChanges();
}

