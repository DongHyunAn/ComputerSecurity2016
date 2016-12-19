using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Sticker.PC.Module.App.RockPaperScissors.Views;
using System;

namespace Sticker.PC.Module.App.RockPaperScissors
{
    public class RockPaperScissorsModule : IModule
    {
        IUnityContainer _unityContainer;

        public RockPaperScissorsModule(IUnityContainer unitycontainer)
        {
            _unityContainer = unitycontainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<object, RPSLogo>(typeof(RPSLogo).Name);
            _unityContainer.RegisterType<object, RPSPrepare>(typeof(RPSPrepare).Name);
            _unityContainer.RegisterType<object, RPSGame>(typeof(RPSGame).Name);
        }
    }
}