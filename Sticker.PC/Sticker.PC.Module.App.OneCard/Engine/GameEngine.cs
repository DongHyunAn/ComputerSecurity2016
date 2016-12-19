using Prism.Events;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Sticker.PC.Module.App.OneCard.Class;
using Sticker.PC.Infra.StaticResources;

namespace Sticker.PC.Module.App.OneCard.Engine
{
    public class GameEngine
    {
        #region Singleton
        private static readonly GameEngine _instance = new GameEngine();

        public static GameEngine getInstance
        {
            get
            {
                return _instance;
            }
        }

        private GameEngine()
        {

        }
        #endregion

        public ObservableCollection<OneCardPlayer> OneCardPlayers { get; set; }


    }
}
