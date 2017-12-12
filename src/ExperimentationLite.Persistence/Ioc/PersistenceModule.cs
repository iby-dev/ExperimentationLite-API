﻿using Autofac;
using Experimentation.Persistence.Repositories;
using ExperimentationLite.Domain;
using ExperimentationLite.Domain.Entities;
using ExperimentationLite.Domain.Repositories;

namespace Experimentation.Persistence.Ioc
{
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().As<IDataContext>().SingleInstance();

            // Dont usually do things this way - was interested in seeing if it would work or not.
            builder.RegisterType<BaseRepository<Feature>>().As<IRepository<Feature,string>>();
            builder.RegisterType<FeaturesRepository>().As<BaseRepository<Feature>>();

            // below is usual way!
            //builder.RegisterType<FeaturesRepository>().As<IRepository<Feature,string>>();
        }
    }
}
