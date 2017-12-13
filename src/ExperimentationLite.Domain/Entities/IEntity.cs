using System;
using LiteDB;

namespace ExperimentationLite.Domain.Entities
{
    public interface IEntity<TKey>
    {
        [BsonId]
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<Guid>
    {
        string Name { get; set; }
        int FriendlyId { get; set; }
    }
}