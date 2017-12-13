using System;
using ExperimentationLite.Configuration;
using ExperimentationLite.Domain;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Experimentation.Persistence
{
    public class DataContext : IDataContext, IDisposable
    {
        public LiteDatabase Database { get; set; }

        public DataContext(IOptions<DataContextSettings> options)
        {
            Database = new LiteDatabase(options.Value.ConnectionString);
        }

        public void Dispose() => Database?.Dispose();
    }
}