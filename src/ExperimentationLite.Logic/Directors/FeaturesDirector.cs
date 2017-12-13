using System;
using System.Collections.Generic;
using Experimentation.Persistence.Repositories;
using ExperimentationLite.Domain.Entities;

namespace ExperimentationLite.Logic.Directors
{
    public class FeaturesDirector : IFeaturesDirector
    {
        private readonly FeaturesRepository _repository;

        public FeaturesDirector(BaseRepository<Feature> repository)
        {
            _repository = repository as FeaturesRepository;
        }

        public IEnumerable<Feature> GetAllFeatures()
        {
            return _repository.GetAll();
        }

        public Feature GetFeatureById(Guid id)
        {
            return _repository.GetById(id);
        }

        public bool FeatureExistsById(Guid id)
        {
            return _repository.Exists(id);
        }

        public Feature GetFeatureByName(string name)
        {
            return _repository.GetByName(name);
        }

        public Feature GetFeatureByFriendlyId(int id)
        {
            return _repository.GetByFriendlyId(id);
        }

        public Guid AddNewFeature(Feature toAdd)
        {
            return _repository.Save(toAdd);
        }

        public bool UpdateFeature(Feature toUpdate)
        {
             return _repository.Update(toUpdate);
        }

        public bool DeleteFeature(Guid id)
        {
            return _repository.Delete(id);
        }
    }
}
