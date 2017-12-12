using ExperimentationLite.Domain.Entities;
using ExperimentationLite.Logic.ViewModels;

namespace ExperimentationLite.Logic.Mapper
{
    public class FeatureViewModelMapper : IDtoToEntityMapper<BaseFeatureViewModel, Feature>
    {
        public Feature Map(BaseFeatureViewModel model)
        {
            return new Feature
            {
                Name = model.Name,
                FriendlyId = model.FriendlyId,
                BucketList = model.BucketList,
            };
        }
    }
}