using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.App.OneCard.Views;
using System;

namespace Sticker.PC.Module.App.OneCard
{
    public class OneCardModule : IModule
    {
        IUnityContainer _unityContainer;

        public OneCardModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, OneCardLogo>(typeof(OneCardLogo).Name);
            _unityContainer.RegisterType<object, OneCardPrepare>(typeof(OneCardPrepare).Name);
            _unityContainer.RegisterType<object, OneCardGame>(typeof(OneCardGame).Name);
        }
    }
}