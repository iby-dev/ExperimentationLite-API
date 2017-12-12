using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Logic.ViewModels
{
    public class ViewModel<TEntity> where TEntity : class, IEntity
    {
        public TEntity Item { get; private set; }

        public ViewModel(TEntity data)
        {
            Item = data;
        }
    }
}