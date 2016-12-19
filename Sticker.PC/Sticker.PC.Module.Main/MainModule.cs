using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.Main.Views;
using System;

namespace Sticker.PC.Module.Main
{
    public class MainModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _container;

        public MainModule(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _container = unityContainer;
        }

        public void Initialize()
        {
            _container.RegisterType<object, AppList>(typeof(AppList).Name);
        }
    }
}