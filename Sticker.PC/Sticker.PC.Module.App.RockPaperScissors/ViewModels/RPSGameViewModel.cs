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
using System.Windows;

namespace Sticker.PC.Module.App.RockPaperScissors.ViewModels
{
    public class RPSGameViewModel : BindableBase, INavigationAware
    {
        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;
        INetworkService _networkService;

        private ObservableCollection<RPSPlayer> _rpsPlayers;
        public ObservableCollection<RPSPlayer> RPSPlayers
        {
            get { return _rpsPlayers; }
            set { SetProperty(ref _rpsPlayers, value); }
        }

        private ObservableCollection<RPSPlayer> _winners = new ObservableCollection<RPSPlayer>();
        public ObservableCollection<RPSPlayer> Winners
        {
            get { return _winners; }
            set { SetProperty(ref _winners, value); }
        }

        private Visibility _popupVisiblity;
        public Visibility PopupVIsibility
        {
            get { return _popupVisiblity; }
            set { SetProperty(ref _popupVisiblity, value); }
        }

        private string _stateMessage;
        public string StateMessage
        {
            get { return _stateMessage; }
            set { SetProperty(ref _stateMessage, value); }
        }

        Dictionary<Player, int> _playersPosition = new Dictionary<Player, int>();

        public RPSGameViewModel(IEventAggregator ea, IRegionManager rm, INetworkService ns)
        {
            _eventAggregator = ea;
            _regionManager = rm;
            _networkService = ns;
        }

        public void init()
        {
            StateMessage = "카드를 올려주세요";

            PopupVIsibility = Visibility.Hidden;

            _playersPosition.Clear();
            RPSPlayers = GameEngine.getInstance.RPSPlayers;

            GameEngine.getInstance.GameStatus = GameEngine.RPSGameStatus.Game;

            for (int i = 0; i < 4; i++)
            {
                if (RPSPlayers[i].GetPlayer() != null)
                {
                    _playersPosition.Add(RPSPlayers[i].GetPlayer(), i);

                    _networkService.SendMessageToPlayer(RPSPlayers[i].GetPlayer(), "RPS_Controller_Controller");

                    RPSPlayers[i].SetSelectedCard(RPSPlayer.RPS.None);
                }
                else
                {
                    continue;
                }
            }

            GameEngine.getInstance.SetPlayerConnectionLossCallBack(playerConnectionLossCallback);
        }

        private void playerConnectionLossCallback()
        {
            GameStartableChecker();
        }

        private void RPSCommandImply(CommandFromPlayerParam obj)
        {
            string[] tokenize = obj.Message.Split('_');

            switch (tokenize[1])
            {
                case "Exit":
                    {
                        if(obj.Sender.Type==Player.PlayerType.Master)
                        {
                            GameEngine.getInstance.finishGameEngine(false);
                        }
                    }break;
                case "Card":
                    {
                        if(RPSPlayers[_playersPosition[obj.Sender]].GetPlayer() != null)
                        {
                            switch (tokenize[2])
                            {
                                case "R": RPSPlayers[_playersPosition[obj.Sender]].SetSelectedCard(RPSPlayer.RPS.Rock); break;
                                case "P": RPSPlayers[_playersPosition[obj.Sender]].SetSelectedCard(RPSPlayer.RPS.Paper); break;
                                case "S": RPSPlayers[_playersPosition[obj.Sender]].SetSelectedCard(RPSPlayer.RPS.Scissors); break;
                                default:
                                    break;
                            }

                            GameStartableChecker();
                        }
                    }
                    break;
                case "Replay":
                    {
                        if (GameEngine.getInstance.GameStatus != GameEngine.RPSGameStatus.None)
                        {
                            this.init();
                        }
                    }break;
                default:
                    break;
            }
        }

        private void GameStartableChecker()
        {
            for (int i = 0; i < 4; i++)
            {
                if (RPSPlayers[i].GetPlayer() != null && RPSPlayers[i].RpsType == RPSPlayer.RPS.None)
                {
                    return;
                }
            }

            GameEngine.getInstance.UnsetPlayerConnectionLossCallBack();
            ThreadPool.QueueUserWorkItem(GameResultPresenter);
        }

        private void GameResultPresenter(object state)
        {
            int time = 3;

            StateMessage = "결과를 발표합니다...";

            Thread.Sleep(TimeSpan.FromSeconds(2));

            while (time >= 1)
            {
                StateMessage = time.ToString();
                time--;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            bool[] rpsChecker = new bool[3];
            for(int i=0;i<3;i++)
            {
                rpsChecker[i] = false;
            }

            foreach(RPSPlayer player in RPSPlayers)
            {
                if(player.RpsType!=RPSPlayer.RPS.None)
                {
                    player.SetBackCardVisibleHidden();
                    rpsChecker[(int)player.RpsType] = true;
                }
            }

            int counter = 0;
            for(int i=0;i<3;i++)
            {
                if(rpsChecker[i])
                {
                    counter++;
                }
            }

            if(counter != 2)
            {
                StateMessage = "비겼습니다! 게임을 다시 시작합니다...";
                Thread.Sleep(TimeSpan.FromSeconds(3));

                init();
            }
            else
            {
                RPSPlayer.RPS winnerType;

                if(!rpsChecker[0])
                {
                    winnerType = RPSPlayer.RPS.Scissors;
                }
                else if(!rpsChecker[1])
                {
                    winnerType = RPSPlayer.RPS.Rock;
                }
                else
                {
                    winnerType = RPSPlayer.RPS.Paper;
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    Winners.Clear();

                    foreach (RPSPlayer player in RPSPlayers)
                    {
                        if (player.RpsType == winnerType)
                        {
                            Winners.Add(player);
                        }
                    }
                }));

                Thread.Sleep(TimeSpan.FromSeconds(3));

                if(Winners.Count==0)
                {
                    _eventAggregator.GetEvent<NotifyToastEvent>().Publish("플레이어의 연결 문제로 게임 진행이 불가합니다. 네트워크 연결을 확인해 주세요.");
                    GameEngine.getInstance.finishGameEngine(false);
                }
                else
                {
                    PopupVIsibility = Visibility.Visible;
                    _networkService.SendMessageToPlayer(_networkService.GetMasterPlayer(), "RPS_Controller_Popup");
                }
            }
        }

        #region INavigationAware implements
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Unsubscribe(RPSCommandImply);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            init();
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Subscribe(RPSCommandImply);
        } 
        #endregion
    }
}
