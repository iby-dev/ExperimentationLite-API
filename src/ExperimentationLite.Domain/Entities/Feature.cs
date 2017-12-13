using System;
using System.Collections.Generic;
using LiteDB;

namespace ExperimentationLite.Domain.Entities
{
    public class Feature : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public int FriendlyId { get; set; }
        public string Name { get; set; }
        public List<string> BucketList { get; set; } = new List<string>();
    }
}