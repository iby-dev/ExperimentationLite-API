using System;
using System.Collections.Generic;
using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Domain.Repositories
{
    public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
    {
        TEntity GetById(Guid idToLookFor);
        TEntity GetByFriendlyId(int idToLookFor);
        bool Exists(Guid idToLookFor);
        bool Exists(int idToLookFor);
        Guid Save(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(Guid id);
        IEnumerable<TEntity> GetAll();
    }
}