using LiteDB;

namespace ExperimentationLite.Domain
{
    public interface IDataContext
    {
        LiteDatabase Database { get; set; }
    }
}