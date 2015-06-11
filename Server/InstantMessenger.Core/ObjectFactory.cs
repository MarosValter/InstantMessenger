using Castle.Windsor;
using Castle.Windsor.Installer;

namespace InstantMessenger.Core
{
    public static class ObjectFactory
    {
        private static IWindsorContainer _container;

        public static void Init()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
        }

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
