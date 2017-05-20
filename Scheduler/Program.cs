namespace Scheduler
{
    using Core.Interfaces.Repositories;
    using Core.Tools;
    using Microsoft.Practices.Unity;
    using Services.DomainServices;

    class Program
    {
        static void Main()
        {
            var unityContaner = IocConfig.GetConfiguredContainer();
            unityContaner.RegisterType<IMissionRepository>(new InjectionFactory((c) => null));
            unityContaner.RegisterType<IRatingRepository>(new InjectionFactory((c) => null));
            unityContaner.RegisterType<IAppCountersRepository>(new InjectionFactory((c) => null));
            var service = unityContaner.Resolve<UserService>();
            service.DecreaseKindActionScales().Wait();
            service.UpdateLastRatingsPlaces().Wait();
        }
    }
}
