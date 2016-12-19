using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.App.Music.Views;
using System;

namespace Sticker.PC.Module.App.Music
{
    public class MusicModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _unityContainer;

        public MusicModule(RegionManager regionManager, IUnityContainer unitycontainer)
        {
            _regionManager = regionManager;
            _unityContainer = unitycontainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, MusicMain>(typeof(MusicMain).Name);
        }
    }
}