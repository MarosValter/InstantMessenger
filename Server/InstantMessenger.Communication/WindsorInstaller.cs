using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace InstantMessenger.Communication
{
    class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMainRequestHandler>().ImplementedBy<MainRequestHandler>().LifeStyle.Transient,
                Component.For<ITransportObjectProcessor>().ImplementedBy<TransportObjectProcessor>().LifeStyle.Transient
            );
        }
    }
}
