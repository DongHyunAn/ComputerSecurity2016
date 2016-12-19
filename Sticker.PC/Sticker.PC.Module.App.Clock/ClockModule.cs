using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.App.Clock.Views;
using System;

namespace Sticker.PC.Module.App.Clock
{
    public class ClockModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _unityContainer;

        public ClockModule(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this._regionManager = regionManager;
            this._unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, ClockMusic>(typeof(ClockMusic).Name);
        }
    }
}