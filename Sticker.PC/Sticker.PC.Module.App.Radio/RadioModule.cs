using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.App.Radio.Views;
using System;

namespace Sticker.PC.Module.App.Radio
{
    public class RadioModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _unityContainer;

        public RadioModule(RegionManager regionManager, IUnityContainer unitycontainer)
        {
            _regionManager = regionManager;
            _unityContainer = unitycontainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, RadioMain>(typeof(RadioMain).Name);
        }
    }
}