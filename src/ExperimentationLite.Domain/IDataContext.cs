using MongoDB.Driver;

namespace ExperimentationLite.Domain
{
    public interface IDataContext
    {
        IMongoDatabase Database { get; set; }
    }
}