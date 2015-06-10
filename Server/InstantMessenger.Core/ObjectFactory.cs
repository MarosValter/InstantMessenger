using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace InstantMessenger.Core
{
    public static class ObjectFactory
    {
        private static IWindsorContainer _container;

        //private static ISessionFactory _sessionFactory;
        //private static readonly IDictionary<Type, object> _bros = new Dictionary<Type, object>();

        public static void Init()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());

            //var assembly = Assembly.Load("InstantMessenger.DataModel");

            //// Load BDO mappings
            //_sessionFactory = Fluently.Configure()
            //    .Database(MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey("Default")).ShowSql)
            //    .Mappings(m => m.FluentMappings.AddFromAssembly(assembly))
            //    .BuildSessionFactory();

            //// Load all BROs
            //var bros = assembly.ExportedTypes.Where(x => x.BaseType != null && 
            //                                             x.BaseType.IsGenericType && 
            //                                             x.BaseType.GetGenericTypeDefinition() == typeof(BROGeneric<>));
            //foreach (var BRO in bros)
            //{
            //    _bros[BRO] = Activator.CreateInstance(BRO);
            //}         
        }

        //public static ISessionFactory GetSessionFactory<T>()
        //{
        //    _assembly = typeof (T).Assembly;
        //    return GetSessionFactory();

        //    //return Fluently.Configure()
        //    //               .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
        //    //               .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>())
        //    //               .ExposeConfiguration(cfg => _configuration = cfg)
        //    //               .BuildSessionFactory();
        //}

        //public static ISession OpenSession()
        //{
        //    ISession session = (_sessionFactory ?? (_sessionFactory = GetSessionFactory())).OpenSession();
        //    var export = new SchemaExport(_configuration);
        //    export.Execute(true, true, false, session.Connection, null);

        //    return session;
        //}

        //public static ISessionFactory GetSessionFactory()
        //{
        //    return _sessionFactory;
        //}

        public static T GetInstance<T>()
        {
            return _container.Resolve<T>();
        }

        public static T[] GetAllInstances<T>()
        {
            return _container.ResolveAll<T>();
        }
    }
}
