using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.RockPaperScissors.Class;
using Sticker.PC.Module.App.RockPaperScissors.Engine;
using Sticker.PC.Module.App.RockPaperScissors.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.RockPaperScissors.ViewModels
{
    public class RPSPrepareViewModel : BindableBase, INavigationAware
    {
        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;
        INetworkService _networkService;
        
        ObservableCollection<RPSPlayer> _rpsPlayers;
        public ObservableCollection<RPSPlayer> RPSPlayers
        {
            get { return _rpsPlayers; }
            set { SetProperty(ref _rpsPlayers, value); }
        }

        public RPSPrepareViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;
        }

        private void CommandFromPlayerEventImply(CommandFromPlayerParam obj)
        {
            string[] tokenize = obj.Message.Split('_');

            if (obj.Sender.Type == Player.PlayerType.Master)
            {
                switch (tokenize[1])
                {
                    case "Exit": GameEngine.getInstance.finishGameEngine(false); break;
                    case "GameStart":
                        {
                            foreach (RPSPlayer player in RPSPlayers)
                            {
                                if (player.GetPlayer() != null)
                                {
                                    _networkService.SetAppController(player.GetPlayer(), NetworkService.ControllerType.RPSController);
                                }
                            }
                            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("RPSGame", UriKind.Relative));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        bool IsConnectionSuccess;

        private void RefreshPlayerThread(object state)
        {
            for(;;)
            {
                if(IsConnectionSuccess)
                {
                    return;
                }

                GameEngine.getInstance.RefreshRPSPlayer(_networkService.GetPlayers());
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        #region INavigationAware Imply

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            GameEngine.getInstance.GameStatus = GameEngine.RPSGameStatus.Wait;

            RPSPlayers = GameEngine.getInstance.RPSPlayers;

            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Subscribe(CommandFromPlayerEventImply);
            IsConnectionSuccess = false;
            ThreadPool.QueueUserWorkItem(RefreshPlayerThread);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            IsConnectionSuccess = true;
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Unsubscribe(CommandFromPlayerEventImply);
        }
        #endregion

    }
}
