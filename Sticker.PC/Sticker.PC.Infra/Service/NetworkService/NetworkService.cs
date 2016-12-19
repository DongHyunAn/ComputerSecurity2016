using Prism.Events;
using Prism.Regions;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.StaticResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;

namespace Sticker.PC.Infra.Service.NetworkService
{
    public class NetworkService : INetworkService
    {
        #region Variable

        IEventAggregator _eventAggregator;
        IRegionManager _regionManager;

        SocketManager _socketManager;
        ManualResetEvent _addNewPlayerLocked = new ManualResetEvent(true);

        private ObservableCollection<Player> _playerList = new ObservableCollection<Player>();
        public ObservableCollection<Player> GetPlayers()
        {
            return _playerList;
        }

        private class PlayerStateObject
        {
            public Player player = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        public enum ControllerType
        {
            WaitDevice,
            Base,

            RPSWait,
            RPSController,

            OneCardWait,
            OneCardController,

            Music
        }

        #endregion

        #region Constructor

        public NetworkService(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            _socketManager = new SocketManager(NewPlayerConnectedCallback, _eventAggregator);
        } 

        #endregion

        public Player GetMasterPlayer()
        {
            foreach (Player player in _playerList)
            {
                if (player.Type == Player.PlayerType.Master)
                {
                    return player;
                }
            }
            return null;

        }

        private void NewPlayerConnectedCallback(Player player)
        {
            _addNewPlayerLocked.WaitOne();
            _addNewPlayerLocked.Reset();

            Console.WriteLine(player.Nickname + " is added");

            if (this.GetMasterPlayer()==null)
            {
                player.Type = Player.PlayerType.Master;
                ThreadPool.QueueUserWorkItem(PlayersSocketConnectionTest);
            }

            _addNewPlayerLocked.Set();

            _playerList.Add(player);

            PlayerStateObject state = new PlayerStateObject();
            state.player = player;

            player.Socket.BeginReceive(
                    state.buffer,
                    0,
                    PlayerStateObject.BufferSize,
                    0,
                    new AsyncCallback(ReceiveFromPlayerCallback),
                    state);
        }

        public void SendMessageToPlayer(Player player, string message)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(message + "\n");
            try
            {
                player.Socket.BeginSend(byteData, 0, byteData.Length, 0, null, null);
            }
            catch(Exception)
            {
                // Socket 연결 끊긴 상태. 
                PlayerSocketLoss(player);
            }
        }

        private void PlayersSocketConnectionTest(object state)
        {
            while (this._playerList.Count != 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));

                try
                {
                    foreach (Player player in _playerList)
                    {
                        SendMessageToPlayer(player, "Confirm_Are You Still There?");
                    }
                }
                catch { }
                // 루프 도중 컬렉션 변경되면 열거자 에러 발생
            }
        }

        private void ReceiveFromPlayerCallback(IAsyncResult ar)
        {
            PlayerStateObject state = (PlayerStateObject)ar.AsyncState;
            Socket handler = state.player.Socket;
            int bytesRead = 0;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                PlayerSocketLoss(state.player);
                return;
            }

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                string content = state.sb.ToString();

                if (content.IndexOf("\n") > -1)
                {
                    content = content.Replace("\n", "");

                    ReceiveSuccessFromPlayer(state.player, content);

                    state.sb.Clear();
                    handler.BeginReceive(state.buffer, 0, PlayerStateObject.BufferSize, 0, new AsyncCallback(ReceiveFromPlayerCallback), state);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, PlayerStateObject.BufferSize, 0, new AsyncCallback(ReceiveFromPlayerCallback), state);
                }
            }
        }

        private void ReceiveSuccessFromPlayer(Player player, string content)
        {
            // 임의의 Player로부터 정상적으로 응답을 받은 경우의 처리
            string[] tokenize = content.Split('_');

            switch (tokenize[0])
            {
                case "Sticker":
                    {
                        if (player.Type == Player.PlayerType.Master)
                        {
                            switch (tokenize[1])
                            {
                                // 조작을 통한 명령
                                case "Command":
                                    {
                                        KeyEvent.KeyType keyParameter;

                                        switch (tokenize[2])
                                        {
                                            case "l": keyParameter = KeyEvent.KeyType.LEFT; break;
                                            case "r": keyParameter = KeyEvent.KeyType.RIGHT; break;
                                            case "u": keyParameter = KeyEvent.KeyType.UP; break;
                                            case "d": keyParameter = KeyEvent.KeyType.DOWN; break;
                                            case "s": keyParameter = KeyEvent.KeyType.SELECT; break;
                                            case "c": keyParameter = KeyEvent.KeyType.CANCEL; break;
                                            default: return;
                                        }

                                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            _eventAggregator.GetEvent<MasterKeyDownEvent>().Publish(keyParameter);
                                        }));
                                    }
                                    break;
                                case "Text":
                                    {
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            _eventAggregator.GetEvent<ReceiveTextDataEvent>().Publish(tokenize[2]);
                                        }));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }break;

                case "Music":
                    {
                        KeyEvent.MusicKeyType keyMusicParameter;
                        switch (tokenize[1])
                        {
                            case "p": keyMusicParameter = KeyEvent.MusicKeyType.PLAY; break;
                            case "st": keyMusicParameter = KeyEvent.MusicKeyType.STOP; break;
                            case "b": keyMusicParameter = KeyEvent.MusicKeyType.BEFORE; break;
                            case "n": keyMusicParameter = KeyEvent.MusicKeyType.NEXT; break;
                            case "su": keyMusicParameter = KeyEvent.MusicKeyType.SHUFFLE; break;
                            case "lu": keyMusicParameter = KeyEvent.MusicKeyType.LISTUP; break;
                            case "ld": keyMusicParameter = KeyEvent.MusicKeyType.LISTDOWN; break;
                            case "r": keyMusicParameter = KeyEvent.MusicKeyType.REPEAT; break;
                            case "seekbar": _eventAggregator.GetEvent<MusicControllerKeyEvent>().Publish(tokenize[2] + "_" + tokenize[3]); return;
                            default: return;
                        }
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _eventAggregator.GetEvent<MusicKeYDownEvent>().Publish(keyMusicParameter);
                        }));
                    }
                    break;
                case "RPS":
                case "OneCard":
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _eventAggregator.GetEvent<CommandFromPlayerEvent>().Publish(new CommandFromPlayerParam() {
                                Message = content,
                                Sender = player});
                        }));
                    }
                    break;
                case "Player":
                    {
                        if(tokenize[1]== "connectionClose")
                        {
                            this.PlayerSocketLoss(player);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void PlayerSocketLoss(Player player)
        {
            if(player!=null)
            {
                switch (player.Type)
                {
                    case Player.PlayerType.Master:
                        {
                            _regionManager.RequestNavigate(RegionName.MainRegion, new Uri("WaitDevice", UriKind.Relative));

                            _eventAggregator.GetEvent<NotifyToastEvent>().Publish("Master와의 연결이 해제되었습니다. 새로운 장치를 기다립니다.");
                            _eventAggregator.GetEvent<MasterConnectionLossEvent>().Publish("Master 연결 끊김");

                            this._playerList.Clear();
                        }
                        break;
                    case Player.PlayerType.Player:
                        {
                            _playerList.Remove(player);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetAppController(Player player, ControllerType controller)
        {
            string message = "Request_Navigate_" + controller.ToString();
            this.SendMessageToPlayer(player, message);
        }
    }
}
