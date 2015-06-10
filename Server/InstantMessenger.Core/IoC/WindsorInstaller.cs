using System;
using System.Reflection;
using AutoMapper;
using AutoMapper.Mappers;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.Base.Implementation;
using InstantMessenger.Core.Repository;
using InstantMessenger.Core.UOW;
using NHibernate;

namespace InstantMessenger.Core.IoC
{
    public class WindsorInstaller : IWindsorInstaller
    {
        private static readonly Assembly DataModelAssembly = Assembly.Load("InstantMessenger.DataModel");

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.ComponentRegistered += KernelOnComponentRegistered;
            container.Kernel.DependencyResolving += KernelOnDependencyResolving;
            container.Kernel.ComponentCreated += KernelOnComponentCreated;
            
            container.Register(
                Component.For<ISessionFactory>().UsingFactoryMethod(CreateSessionFactory).LifeStyle.Singleton,
                Component.For<UnitOfWorkInterceptor>().LifeStyle.Transient,
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.PerThread,
                Component.For<IConfiguration>().UsingFactoryMethod(CreateAutoMapperConfiguration).LifeStyle.Singleton,
                Component.For<IRepository<IBDO>>().ImplementedBy<Repository<IBDO>>().LifeStyle.Transient,

                Classes.FromAssembly(DataModelAssembly).BasedOn(typeof(BROGeneric<>)).LifestyleSingleton(),
                Classes.FromAssembly(DataModelAssembly).BasedOn<IDataManager>().WithService.DefaultInterfaces().LifestyleSingleton()
            );
        }

        private void KernelOnComponentCreated(ComponentModel model, object instance)
        {

        }

        private void KernelOnDependencyResolving(ComponentModel client, DependencyModel model, object dependency)
        {

        }

        private void KernelOnComponentRegistered(string key, IHandler handler)
        {
            if (UnitOfWorkHelper.IsRepositoryClass(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
                return;
            }

            foreach (var method in handler.ComponentModel.Implementation.GetMethods())
            {
                if (UnitOfWorkHelper.HasUnitOfWorkAttribute(method))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
                    return;
                }
            }
        }

        #region Factory methods

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey("Default")).ShowSql)
                .Mappings(m => m.FluentMappings.AddFromAssembly(DataModelAssembly))
                .BuildSessionFactory();
        }

        private static IConfiguration CreateAutoMapperConfiguration()
        {
            return new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
        }

        #endregion
    }
}
