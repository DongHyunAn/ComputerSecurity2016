using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.Gallery.Views;
using System;

namespace Sticker.PC.Module.App.Gallery
{
    public class GalleryModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _unityContainer;

        public GalleryModule(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this._regionManager = regionManager;
            this._unityContainer = unityContainer; 
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, GalleryMain>(typeof(GalleryMain).Name);
            _unityContainer.RegisterType<object, GalleryDetail>(typeof(GalleryDetail).Name);
        }
    }
}