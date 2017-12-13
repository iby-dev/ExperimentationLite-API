using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExperimentationLite.Domain.Entities;
using ExperimentationLite.Domain.Exceptions;
using ExperimentationLite.Domain.Repositories;
using LiteDB;
using Polly;

namespace Experimentation.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : IEntity
    {
        protected abstract LiteCollection<TEntity> Collection { get; }

        public virtual TEntity GetById(Guid idToLookFor)
        {
            return Retry(() => Collection.FindById(idToLookFor));
        }

        public virtual TEntity GetByFriendlyId(int idToLookFor)
        {
            return Retry(() => Collection.FindOne(x => x.FriendlyId.Equals(idToLookFor)));
        }

        public virtual bool Exists(Guid idToLookFor)
        {
            return Retry(() =>Collection.Exists(x => x.Id.Equals(idToLookFor)));
        }

        public virtual bool Exists(int idToLookFor)
        {
            return Retry(() => Collection.Exists(x => x.FriendlyId.Equals(idToLookFor)));
        }

        public virtual Guid Save(TEntity entity)
        {
            var isUniqueId = Collection.Count(item => item.FriendlyId == entity.FriendlyId);
            if (isUniqueId > 0)
            {
                throw new NonUniqueValueDetectedException(GetType().FullName, entity.FriendlyId.ToString());
            }

            var isUniqueName = Collection.Count(item => item.Name == entity.Name);
            if (isUniqueName > 0)
            {
                throw new NonUniqueValueDetectedException(GetType().FullName, entity.Name);
            }

            return Retry(() => Collection.Insert(entity));
        }

        public bool Update(TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id.ToString()))
            {
                throw new ArgumentNullException($"{nameof(entity.Id)}", "The given entity does not have an id set on it.");
            }

            var original = GetById(entity.Id);
            if (original.FriendlyId != entity.FriendlyId) // Friendly id has changed from original.
            {
                var isUniqueId = Collection.Count(item => item.FriendlyId == entity.FriendlyId);
                if (isUniqueId > 0)
                {
                    throw new NonUniqueValueDetectedException(GetType().FullName, entity.FriendlyId.ToString());
                }
            }

            if (original.Name != entity.Name) // name has changed from original.
            {
                var isUniqueName = Collection.Count(item => item.Name == entity.Name);
                if (isUniqueName > 0)
                {
                    throw new NonUniqueValueDetectedException(GetType().FullName, entity.Name);
                }
            }

            return Retry(()  => Collection.Update(entity));
        }

        public bool Delete(Guid id)
        {
            return Retry(() => Collection.Delete(id));
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Retry(() => Collection.FindAll());
        }

        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<LiteException>(i => i.InnerException.GetType() == typeof(Exception))
                .Retry(3)
                .Execute(action);
        }
    }
}