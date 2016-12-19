using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.Service.NetworkService;
using Sticker.PC.Infra.StaticResources;
using Sticker.PC.Module.App.OneCard.Class;
using Sticker.PC.Module.App.OneCard.Controller;
using Sticker.PC.Module.App.OneCard.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sticker.PC.Module.App.OneCard.ViewModels
{
    public class OneCardGameViewModel : BindableBase, INavigationAware
    {
        #region property and variable

        CanvasController _canvasController;
        public CanvasController CanvasCont { get { return _canvasController; } }

        private string _notifyText;
        public string NotifyText
        {
            get { return _notifyText; }
            set { SetProperty(ref _notifyText, value); }
        }

        IRegionManager _regionManager;
        INetworkService _networkService;
        IEventAggregator _eventAggregator;

        ObservableCollection<OneCardPlayer> _oneCardPlayers;
        public ObservableCollection<OneCardPlayer> OneCardPlayers
        {
            get { return _oneCardPlayers; }
            set { SetProperty(ref _oneCardPlayers, value); }
        }

        private int _penaltyCardNum;
        public int PenaltyCardNum {
            get {
                return _penaltyCardNum;
            }
            set
            {
                _penaltyCardNum = value;
                if (_penaltyCardNum == 0)
                {
                    IsPenaltyShown = false;
                }
                else
                {
                    IsPenaltyShown = true;
                    PenaltyCard = _penaltyCardNum.ToString();
                }
            }
        }

        private string _penaltyCard;
        public string PenaltyCard
        {
            get { return _penaltyCard; }
            set { SetProperty(ref _penaltyCard, value); }
        }

        private bool _isPenaltyShown;
        public bool IsPenaltyShown
        {
            get { return _isPenaltyShown; }
            set { SetProperty(ref _isPenaltyShown, value); }
        }

        #endregion

        public OneCardGameViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, INetworkService networkService)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _networkService = networkService;
        }

        public void OneCardNotifyMessage(string message)
        {
            NotifyText = "";
            NotifyText = message;
        }

        ManualResetEvent _resetEvent;

        private void OneCardGameViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Object obj in e.OldItems)
                {
                    if (obj is Player)
                    {
                        Player player = obj as Player;

                        OneCardPlayer gamePlayer = SearchOneCardPlayer(player);

                        if(gamePlayer!=null)
                        {
                            _resetEvent.WaitOne();

                            gamePlayer.PreviousPlayer.NextPlayer = gamePlayer.NextPlayer;
                            gamePlayer.NextPlayer.PreviousPlayer = gamePlayer.PreviousPlayer;

                            if(gamePlayer.NextPlayer == gamePlayer.PreviousPlayer)
                            {
                                _eventAggregator.GetEvent<NotifyToastEvent>().Publish(player.Nickname + "님이 게임을 떠나 게임의 인원 수가 부족합니다. 게임을 종료합니다.");

                                gamePlayer.init();

                                this.finish();
                                return;
                            }

                            OneCardNotifyMessage(player.Nickname + "님이 게임을 떠났습니다.");

                            ThreadPool.QueueUserWorkItem(PlayerOutThread, gamePlayer);

                            if (_nowTurnPlayer == gamePlayer)
                            {
                                ThreadPool.QueueUserWorkItem(GameTurnHost, gamePlayer.NextPlayer);
                            }
                        }
                    }
                }
            }
        }

        private void PlayerOutThread(object state)
        {
            if(state is OneCardPlayer)
            {
                OneCardPlayer player = state as OneCardPlayer;

                for (int i = 0; i < player.Cards.Count; i++)
                {
                    _deck.addToGrave(player.Cards[i]);
                    //_canvasController.SendCard((CanvasController.Direction)OneCardPlayers.IndexOf(player), "bc", true);
                    //Thread.Sleep(TimeSpan.FromSeconds(0.2));
                }

                player.init();
            }
        }

        public OneCardPlayer SearchOneCardPlayer(Player player)
        {
            foreach (OneCardPlayer gamePlayer in OneCardPlayers)
            {
                if (gamePlayer.Player == player)
                {
                    return gamePlayer;
                }
            }

            return null;
        }

        #region INavigationAware imply

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Unsubscribe(CommandFromPlayerEventImply);
            _networkService.GetPlayers().CollectionChanged -= OneCardGameViewModel_CollectionChanged;
            _canvasController.finish();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _canvasController = new CanvasController((UserControl)_regionManager.Regions[RegionName.MainRegion].ActiveViews.FirstOrDefault());

            OneCardPlayers = new ObservableCollection<OneCardPlayer>();
            ObservableCollection<OneCardPlayer> temp = new ObservableCollection<OneCardPlayer>();

            _networkService.GetPlayers().CollectionChanged += OneCardGameViewModel_CollectionChanged;

            IsShapeVisible = false;
            _qInverse = false;
            PenaltyCardNum = 0;

            _resetEvent = new ManualResetEvent(true);

            Random random = new Random();
            for(int i=0;i<4;i++)
            {
                int index = random.Next(0, GameEngine.getInstance.OneCardPlayers.Count);

                OneCardPlayer player = GameEngine.getInstance.OneCardPlayers[index];
                if (player.IsExist)
                {
                    OneCardPlayers.Add(player);
                }
                else
                {
                    temp.Add(player);
                }

                GameEngine.getInstance.OneCardPlayers.RemoveAt(index);
            }

            foreach(OneCardPlayer player in temp)
            {
                OneCardPlayers.Add(player);
            }

            OneCardPlayer previousPlayer = null;
            OneCardPlayer firstPlayer = null;

            int[] position = {0, 2, 1, 3};

            foreach(int index in position)
            {
                if (OneCardPlayers[index].IsExist)
                {
                    if(previousPlayer==null)
                    {
                        previousPlayer = OneCardPlayers[index];
                        firstPlayer = OneCardPlayers[index];
                    }
                    else
                    {
                        previousPlayer.NextPlayer = OneCardPlayers[index];
                        OneCardPlayers[index].PreviousPlayer = previousPlayer;

                        previousPlayer = OneCardPlayers[index];
                    }
                }
            }
            previousPlayer.NextPlayer = firstPlayer;
            firstPlayer.PreviousPlayer = previousPlayer;

            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Subscribe(CommandFromPlayerEventImply);
            
            ThreadPool.QueueUserWorkItem(GameStart);
        }

        #endregion

        bool _qInverse;

        private void finish()
        {
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

            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("AppList", UriKind.Relative));
        }


        private bool _popupVisibility;
        public bool PopupVisibility
        {
            get { return _popupVisibility; }
            set { SetProperty(ref _popupVisibility, value); }
        }

        private Player _player;
        public Player Winner
        {
            get { return _player; }
            set { SetProperty(ref _player, value); }
        }

        public void Win(object state)
        {
            if (state is OneCardPlayer)
            {
                OneCardPlayer player = state as OneCardPlayer;

                Thread.Sleep(TimeSpan.FromSeconds(2));

                Winner = player.Player;
                PopupVisibility = true;

                OneCardNotifyMessage(Winner.Nickname + "님이 승리하셨습니다!!");
                Thread.Sleep(TimeSpan.FromSeconds(4));

                OneCardNotifyMessage("메인 화면으로 나갑니다...");
                Thread.Sleep(TimeSpan.FromSeconds(4));

                this.finish();

                Winner = null;
                PopupVisibility = false;
            }
        }

        private void CommandFromPlayerEventImply(CommandFromPlayerParam obj)
        {
            string[] tokenize = obj.Message.Split('_');

            if (obj.Sender.Type == Player.PlayerType.Master)
            {
                if (tokenize[1] == "Exit")
                {
                    finish();
                    return;
                }
            }

            switch (tokenize[2])
            {
                case "Submit":
                    {
                        IsShapeVisible = false;

                        //"OneCard_Controller_Submit_c7_h"
                        OneCardPlayer player = SearchOneCardPlayer(obj.Sender);
                        _canvasController.SendCard((CanvasController.Direction)OneCardPlayers.IndexOf(player), tokenize[3]);

                        _deck.addToGrave(tokenize[3]);
                        player.Cards.Remove(tokenize[3]);

                        if(player.Cards.Count==0)
                        {
                            ThreadPool.QueueUserWorkItem(Win, player);
                            return;
                        }

                        if(tokenize[3][0]=='j')
                        {
                            OneCardNotifyMessage("조커!");
                            PenaltyCardNum += 10;
                        }else if(tokenize[3][1]=='a')
                        {
                            if(tokenize[3][0]=='s')
                            {
                                PenaltyCardNum += 5;
                            }
                            else
                            {
                                PenaltyCardNum += 3;
                            }
                        }else if(tokenize[3][1]=='2')
                        {
                            PenaltyCardNum += 2;
                        }

                        if (tokenize.Length==5)
                        {
                            // Submit 뒤에, 카드이름 뒤에 뭔가 더 있다면 7을 냈다는 소리임
                            switch(tokenize[4])
                            {
                                case "c": SetShape(Shape.c); break;
                                case "d": SetShape(Shape.d); break;
                                case "h": SetShape(Shape.h); break;
                                case "s": SetShape(Shape.s); break;
                            }

                            _topCard = tokenize[4] + "7";
                        }
                        else
                        {
                            _topCard = tokenize[3];
                        }

                        player.IsMyTurn = false;

                        switch (_topCard[1])
                        {
                            case 'k': player = player.PreviousPlayer; break;
                            case 'q': _qInverse = !_qInverse; break;
                            case 'j': player = player.NextPlayer; break;
                        }

                        if(_qInverse)
                        {
                            ThreadPool.QueueUserWorkItem(GameTurnHost, player.PreviousPlayer);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(GameTurnHost, player.NextPlayer);
                        }
                    }
                    break;
                case "Penalty":
                    {
                        ThreadPool.QueueUserWorkItem(PenaltyThread, SearchOneCardPlayer(obj.Sender));
                    }
                    break;

                default:
                    break;
            }
        }

        private void PenaltyThread(object state)
        {
            _resetEvent.Reset();
            if (state is OneCardPlayer)
            {
                OneCardPlayer player = state as OneCardPlayer;

                CanvasController.Direction dir = (CanvasController.Direction)OneCardPlayers.IndexOf(player);

                if (PenaltyCardNum == 0)
                {
                    getCard(dir);
                    string drawCard = _deck.getCardFromDeck();
                    _networkService.SendMessageToPlayer(player.Player, "OneCard_Controller_drawCard_" + drawCard);
                    AddPlayerCards(player, drawCard);
                }
                else
                {
                    for (int i = 0; i < PenaltyCardNum; i++)
                    {
                        getCard(dir);

                        string drawCard = _deck.getCardFromDeck();
                        _networkService.SendMessageToPlayer(player.Player, "OneCard_Controller_drawCard_" + drawCard);
                        AddPlayerCards(player, drawCard);

                        Thread.Sleep(TimeSpan.FromSeconds(0.2));
                    }
                }

                PenaltyCardNum = 0;
                player.IsMyTurn = false;

                if (_qInverse)
                {
                    ThreadPool.QueueUserWorkItem(GameTurnHost, player.PreviousPlayer);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(GameTurnHost, player.NextPlayer);
                }
            }
            _resetEvent.Set();
        }

        public void getCard(CanvasController.Direction dir)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                this.CanvasCont.GetCard(dir);
            }));
        }

        public void FirstCardOpen(string card)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                this.CanvasCont.FirstCardOpen(card);
            }));
        }

        public void AddPlayerCards(OneCardPlayer player, string card)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                player.Cards.Add(card);
            }));
        }

        Deck _deck;
        string _topCard;

        OneCardPlayer _nowTurnPlayer;

        private void GameStart(object state)
        {
            _resetEvent.Reset();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            this.OneCardNotifyMessage("게임을 준비합니다");

            _deck = new Deck();

            Thread.Sleep(TimeSpan.FromSeconds(1));
            this.OneCardNotifyMessage("5장씩 카드를 나누어 갖습니다");

            for (int i=0;i<4;i++)
            {
                if(OneCardPlayers[i].IsExist)
                {
                    CanvasController.Direction dir = (CanvasController.Direction) i;

                    for(int j=0;j<5;j++)
                    {
                        getCard(dir);
                        Thread.Sleep(TimeSpan.FromSeconds(0.2));

                        string drawCard = _deck.getCardFromDeck();
                        _networkService.SendMessageToPlayer(OneCardPlayers[i].Player, "OneCard_Controller_drawCard_" + drawCard);
                        AddPlayerCards(OneCardPlayers[i], drawCard);
                    }
                }
            }
            _topCard = _deck.getCardFromDeck();
            FirstCardOpen(_topCard);

            _deck.addToGrave(_topCard);

            Thread.Sleep(TimeSpan.FromSeconds(0.8));

            this.OneCardNotifyMessage(OneCardPlayers[0].Player.Nickname + "님 부터 게임을 시작합니다");

            ThreadPool.QueueUserWorkItem(GameTurnHost, OneCardPlayers[0]);
            _resetEvent.Set();
        }

        private void GameTurnHost(object state)
        {
            if(state is OneCardPlayer)
            {
                _nowTurnPlayer = state as OneCardPlayer;
                _nowTurnPlayer.IsMyTurn = true;

                _networkService.SendMessageToPlayer(_nowTurnPlayer.Player, "OneCard_Controller_isYourTurn_" + _topCard + "_" + PenaltyCardNum.ToString());
            }
        }

        enum Shape
        {
            s,
            c,
            d,
            h
        }

        private string _shapeimage;
        public string ShapeImage
        {
            get { return _shapeimage; }
            set { SetProperty(ref _shapeimage, value); }
        }

        private bool _isShapeVisible;
        public bool IsShapeVisible
        {
            get { return _isShapeVisible; }
            set { SetProperty(ref _isShapeVisible, value); }
        }

        private void SetShape(Shape shape)
        {
            IsShapeVisible = true;
            ShapeImage = "/Sticker.PC.Infra;component/Resources/Images/OneCard/Cards/shape_" + shape.ToString() + ".png";
        }
    }
}
