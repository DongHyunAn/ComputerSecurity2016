using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.Initialize.Views;
using System;

namespace Sticker.PC.Module.Initialize
{
    public class InitializeModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _container;

        public InitializeModule(IRegionManager regionManager, IUnityContainer container)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<object, Logo>(typeof(Logo).Name);
            _container.RegisterType<object, CreateProfile>(typeof(CreateProfile).Name);
            _container.RegisterType<object, WaitDevice>(typeof(WaitDevice).Name);

            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("Logo", UriKind.Relative));
        }
    }
}