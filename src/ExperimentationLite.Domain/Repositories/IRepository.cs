using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Domain.Repositories
{
    public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);
        Task<bool> Exists(TKey id);
        Task<TEntity> GetByFriendlyIdAsync(int id);
        Task<TEntity> SaveAsync(TEntity entity);
        Task DeleteAsync(TKey id);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task UpdateAsync(TEntity entity);
    }
}