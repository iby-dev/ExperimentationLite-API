using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExperimentationLite.Domain.Entities
{
    public interface IEntity<TKey>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<string>
    {
        string Name { get; set; }
        int FriendlyId { get; set; }
    }
}