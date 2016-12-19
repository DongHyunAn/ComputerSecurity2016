using Prism.Events;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.RockPaperScissors.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Module.App.RockPaperScissors.Engine
{
    public class GameEngine
    {
        #region Singleton
        public static GameEngine _instance = new GameEngine();

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

        public enum RPSGameStatus
        {
            None,
            Wait,
            Game,
            End
        }

        public RPSGameStatus GameStatus { get; set; }

        public ObservableCollection<RPSPlayer> RPSPlayers { get; set; }

        INetworkService _networkService;
        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;

        private void MasterConnectionLossEventImply(string obj)
        {
            GameEngine.getInstance.finishGameEngine(true);
        }

        public void initGameEngine(INetworkService networkService, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _networkService = networkService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<MasterConnectionLossEvent>().Subscribe(MasterConnectionLossEventImply);

            GameStatus = GameEngine.RPSGameStatus.None;

            RPSPlayers = new ObservableCollection<RPSPlayer>();
            for (int i = 0; i < 4; i++)
            {
                RPSPlayers.Add(new RPSPlayer());
            }
            
            _networkService.GetPlayers().CollectionChanged += playersCollectionChanged;

            RefreshRPSPlayer(_networkService.GetPlayers());
        }

        public void finishGameEngine(bool isUnExpectFinish)
        {
            _networkService.GetPlayers().CollectionChanged -= playersCollectionChanged;
            _eventAggregator.GetEvent<MasterConnectionLossEvent>().Unsubscribe(MasterConnectionLossEventImply);

            playerConnectionLossCallBack = null;

            GameStatus = GameEngine.RPSGameStatus.End;

            foreach (RPSPlayer player in RPSPlayers)
            {
                if (player.GetPlayer() != null)
                {
                    switch (player.GetPlayer().Type)
                    {
                        case Player.PlayerType.Master:
                            _networkService.SetAppController(player.GetPlayer(), NetworkService.ControllerType.Base);
                            break;
                        case Player.PlayerType.Player:
                            _networkService.SetAppController(player.GetPlayer(), NetworkService.ControllerType.WaitDevice);
                            break;
                        default:
                            break;
                    }
                }

                player.init();
            }

            if(!isUnExpectFinish)
            {
                _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
            }
        }

        private void playersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (GameStatus)
            {
                case RPSGameStatus.None:
                case RPSGameStatus.Wait:
                    {
                        RefreshRPSPlayer(sender as ObservableCollection<Player>);
                    }
                    break;
                case RPSGameStatus.Game:
                    {
                        String name="";

                        if(e.OldItems!=null)
                        {
                            foreach(Object obj in e.OldItems)
                            {
                                if(obj is Player)
                                {
                                    Player player = obj as Player;

                                    foreach(RPSPlayer gamePlayer in RPSPlayers)
                                    {
                                        if(gamePlayer.GetPlayer() == player)
                                        {
                                            name = player.Nickname;
                                            gamePlayer.SetPlayer(null);

                                            CallPlayerConnectionLossCallBack();

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // CollectionChange가 일어나면, 사용자 수를 검사

                        int count = 0;
                        foreach(RPSPlayer player in RPSPlayers)
                        {
                            if(player.GetPlayer()!=null)
                            {
                                count++;
                            }
                        }

                        if(count<=1)
                        {
                            if(_networkService.GetMasterPlayer()==null)
                            {
                                finishGameEngine(true);
                            }
                            else
                            {
                                finishGameEngine(false);
                                _eventAggregator.GetEvent<NotifyToastEvent>().Publish(name + " 유저와의 연결이 종료되어 플레이어가 부족합니다. 게임을 종료합니다.");
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void RefreshRPSPlayer(ObservableCollection<Player> players)
        {
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                Player player;

                try
                {
                    player = players[i];
                    _networkService.SetAppController(player, NetworkService.ControllerType.RPSWait);
                    count++;
                }
                catch (Exception)
                {
                    player = null;
                }

                RPSPlayers[i].SetPlayer(player);
            }

            if (count >= 2)
            {
                _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "RPS_Wait_Ready");
            }
            else
            {
                _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "RPS_Wait_Wait");
            }
        }

        Action playerConnectionLossCallBack = null;

        public void SetPlayerConnectionLossCallBack(Action callback)
        {
            playerConnectionLossCallBack = callback;
        }

        private void CallPlayerConnectionLossCallBack()
        {
            playerConnectionLossCallBack?.Invoke();
        }

        public void UnsetPlayerConnectionLossCallBack()
        {
            playerConnectionLossCallBack = null;
        }
    }
}
