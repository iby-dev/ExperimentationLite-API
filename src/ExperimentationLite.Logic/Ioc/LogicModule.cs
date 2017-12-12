using Autofac;
using ExperimentationLite.Domain.Entities;
using ExperimentationLite.Logic.Directors;
using ExperimentationLite.Logic.Mapper;
using ExperimentationLite.Logic.ViewModels;

namespace ExperimentationLite.Logic.Ioc
{
    public class LogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FeaturesDirector>().As<IFeaturesDirector>();
            builder.RegisterType<FeatureViewModelMapper>().As<IDtoToEntityMapper<BaseFeatureViewModel, Feature>>();
        }
    }
}