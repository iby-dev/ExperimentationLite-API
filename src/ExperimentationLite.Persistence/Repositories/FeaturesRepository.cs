using ExperimentationLite.Domain;
using ExperimentationLite.Domain.Entities;
using LiteDB;

namespace Experimentation.Persistence.Repositories
{
    public class FeaturesRepository : BaseRepository<Feature> 
    {
        private const string CollectionName = "Features";

        private readonly IDataContext _ctx;

        protected override LiteCollection<Feature> Collection => _ctx.Database.GetCollection<Feature>(CollectionName);

        public FeaturesRepository(IDataContext context)
        {
            _ctx = context;

            if (Collection != null)
            {
                Collection.EnsureIndex(x => x.FriendlyId);
                Collection.EnsureIndex(x => x.Name);
            }
        }

        public Feature GetByName(string name)
        {
            return Retry(() => Collection.FindOne(x => x.Name.Equals(name)));
        }
    }
}