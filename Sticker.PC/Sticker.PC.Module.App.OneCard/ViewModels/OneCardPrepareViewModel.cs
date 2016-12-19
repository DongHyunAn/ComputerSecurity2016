using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.OneCard.Class;
using Sticker.PC.Module.App.OneCard.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Sticker.PC.Module.App.OneCard.ViewModels
{
    public class OneCardPrepareViewModel : BindableBase, INavigationAware
    {
        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;
        INetworkService _networkService;

        ObservableCollection<OneCardPlayer> _oneCardPlayers;
        public ObservableCollection<OneCardPlayer> OneCardPlayers
        {
            get { return _oneCardPlayers; }
            set { SetProperty(ref _oneCardPlayers, value); }
        }

        public OneCardPrepareViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
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
                    case "Exit": this.finish(false); break;
                    case "GameStart":
                        {
                            IsConnectionSuccess = true;
                            _networkService.GetPlayers().CollectionChanged -= playersCollectionChanged;

                            GameEngine.getInstance.OneCardPlayers = this.OneCardPlayers;

                            foreach (OneCardPlayer player in OneCardPlayers)
                            {
                                if (player.Player != null)
                                {
                                    _networkService.SetAppController(player.Player, NetworkService.ControllerType.OneCardController);
                                }
                            }
                            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("OneCardGame", UriKind.Relative));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void MasterConnectionLossEventImply(string obj)
        {
            this.finish(true);
        }

        private void playersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(sender is ObservableCollection<Player>)
            {
                RefreshPlayerList(sender as ObservableCollection<Player>);
            }
        }

        private void RefreshPlayerList(ObservableCollection<Player> players)
        {
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                Player player;

                try
                {
                    player = players[i];
                    _networkService.SetAppController(player, NetworkService.ControllerType.OneCardWait);
                    count++;
                }
                catch (Exception)
                {
                    player = null;
                }

                try
                {
                    OneCardPlayers[i].SetPlayer(player);
                }
                catch
                {
                    return;
                }
            }

            if (count >= 2)
            {
                _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "OneCard_Wait_Ready");
            }
            else
            {
                _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "OneCard_Wait_Wait");
            }
        }

        bool IsConnectionSuccess;

        private void RefreshPlayerThread(object state)
        {
            for (;;)
            {
                if (IsConnectionSuccess)
                {
                    return;
                }

                this.RefreshPlayerList(_networkService.GetPlayers());
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        public void finish(bool isUnExpectFinish)
        {
            IsConnectionSuccess = true;
            _networkService.GetPlayers().CollectionChanged -= playersCollectionChanged;

            foreach (OneCardPlayer player in OneCardPlayers)
            {
                if (player.Player != null)
                {
                    switch (player.Player.Type)
                    {
                        case Player.PlayerType.Master:
                            _networkService.SetAppController(player.Player, NetworkService.ControllerType.Base);
                            break;
                        case Player.PlayerType.Player:
                            _networkService.SetAppController(player.Player, NetworkService.ControllerType.WaitDevice);
                            break;
                        default:
                            break;
                    }
                }

                player.init();
            }

            OneCardPlayers.Clear();

            if (!isUnExpectFinish)
            {
                _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
            }
        }

        #region INavigationAwareImply

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterConnectionLossEvent>().Subscribe(MasterConnectionLossEventImply);
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Subscribe(CommandFromPlayerEventImply);

            _networkService.GetPlayers().CollectionChanged += playersCollectionChanged;

            OneCardPlayers = new ObservableCollection<OneCardPlayer>();

            for (int i = 0; i < 4; i++)
            {
                OneCardPlayers.Add(new OneCardPlayer());
            }

            IsConnectionSuccess = false;
            ThreadPool.QueueUserWorkItem(RefreshPlayerThread);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MasterConnectionLossEvent>().Unsubscribe(MasterConnectionLossEventImply);
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Unsubscribe(CommandFromPlayerEventImply);
        } 

        #endregion
    }
}
