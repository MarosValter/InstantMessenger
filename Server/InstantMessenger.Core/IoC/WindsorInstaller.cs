using System.Reflection;
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
            
            container.Register(
                Component.For<ISessionFactory>().UsingFactoryMethod(CreateSessionFactory).LifeStyle.Singleton,
                Component.For<UnitOfWorkInterceptor>().LifeStyle.Transient,
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.PerThread,
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).LifeStyle.Transient,

                Classes.FromAssembly(DataModelAssembly).BasedOn(typeof(BROGeneric<>)).LifestyleSingleton(),
                Classes.FromAssembly(DataModelAssembly).BasedOn<DataManagerBase>()
                                                       .WithService.DefaultInterfaces()
                                                       .WithService.Self()
                                                       .LifestyleSingleton(),
                Classes.FromAssembly(DataModelAssembly).BasedOn<IBFO>()
                                                       .LifestyleSingleton()
                                                       .WithServiceDefaultInterfaces()
                                                       .WithServiceSelf()
            );
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
                if (UnitOfWorkHelper.HasAttribute(method, typeof(UnitOfWorkAttribute)))
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

        #endregion
    }
}
