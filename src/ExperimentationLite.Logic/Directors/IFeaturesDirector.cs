using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Logic.Directors
{
    public interface IFeaturesDirector
    {
        IEnumerable<Feature> GetAllFeatures();
        Feature GetFeatureById(Guid id);
        bool FeatureExistsById(Guid id);
        Feature GetFeatureByName(string name);
        Feature GetFeatureByFriendlyId(int id);
        Guid AddNewFeature(Feature toAdd);
        bool UpdateFeature(Feature toUpdate);
        bool DeleteFeature(Guid id);
    }
}