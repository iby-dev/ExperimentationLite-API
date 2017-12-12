using System.Collections.Generic;
using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Logic.ViewModels
{
    public class ListViewModel<TEntity> where TEntity : class, IEntity
    {
        public IEnumerable<TEntity> Items { get; private set; }

        public ListViewModel(IEnumerable<TEntity> data )
        {
            Items = data;
        }
    }
}