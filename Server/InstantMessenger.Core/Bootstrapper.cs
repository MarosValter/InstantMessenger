using System.Threading.Tasks;
using AutoMapper;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.UOW;

namespace InstantMessenger.Core
{
    public static class Bootstrapper
    {
        public static void Init()
        {
            ObjectFactory.Init();
            // Build ISessionFactory
            ObjectFactory.GetInstance<IUnitOfWork>();

            var dataManagers = ObjectFactory.GetAllInstances<IDataManager>();
            Mapper.Initialize(cfg => 
            { 
                Parallel.ForEach(dataManagers, dm => dm.RegisterMapping(cfg));
            });
            
        }
    }
}
