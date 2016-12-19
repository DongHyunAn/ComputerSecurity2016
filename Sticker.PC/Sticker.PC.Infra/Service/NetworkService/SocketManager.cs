using Prism.Events;
using Sticker.PC.Infra.Class;
using Sticker.PC.Infra.Events;
using Sticker.PC.Infra.StaticResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Service.NetworkService
{
    public class SocketManager : IDisposable
    {
        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                foreach(Socket socket in _notConfirmSocketList)
                {
                    this.SocketCloser(socket);
                }

                this.SocketCloser(_listenerSocket);
                this.SocketCloser(_broadcastSocket);

                disposedValue = true;
            }
        }

        ~SocketManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Variable

        public static int _portNum = 19258;
        public static string _applicationID = "Sticker";

        LocalAddressInformation _localNetworkInformation;
        Socket _listenerSocket = null;
        Socket _broadcastSocket = null;

        ManualResetEvent _localAddressInfoFlag;

        Action<Player> _getPlayerCallback = null;
        IEventAggregator _eventAggregator;

        List<Socket> _notConfirmSocketList = new List<Socket>();

        private string ConnnectionStateMessage
        {
            set
            {
                _eventAggregator.GetEvent<NetworkStatusNotifyEvent>().Publish(value);
            }
        }

        #endregion

        public SocketManager(Action<Player> getPlayerCallback, IEventAggregator eventAggregator)
        {
            _getPlayerCallback = getPlayerCallback;
            _eventAggregator = eventAggregator;

            ThreadPool.QueueUserWorkItem(ConnectToPlayersStream);
        }

        private class StateObject
        {
            public Socket socket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        private void ConnectToPlayersStream(object state)
        {
            _localAddressInfoFlag = new ManualResetEvent(false);

            ThreadPool.QueueUserWorkItem(LocalNetworkSetup);

            _localAddressInfoFlag.WaitOne();

            ThreadPool.QueueUserWorkItem(UDPBroadcastInNetwork);
            ThreadPool.QueueUserWorkItem(DeviceWaitUsingTCPSocket);
        }

        private void LocalNetworkSetup(object state)
        {
            int count = 0;

            while (true)
            {
                _localNetworkInformation = new LocalAddressInformation();

                if (_localNetworkInformation.LocalIPAddress != null)
                {
                    _localAddressInfoFlag.Set();
                    break;
                }
                ConnnectionStateMessage = "로컬 네트워크가 확인되지 않습니다. 인터넷 설정을 확인해 주세요.";

                Thread.Sleep(TimeSpan.FromSeconds(5));
                
                // 24 * 5 = 120 s = 2 min
                if (count++ == 24)
                {
                    _eventAggregator.GetEvent<NotifyToastEvent>().Publish("로컬 네트워크를 찾지 못했습니다. 프로그램을 종료합니다.");
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    _eventAggregator.GetEvent<ProgramShutDownEvent>().Publish("network error");
                    break;
                }
            }
        }

        private void UDPBroadcastInNetwork(object state)
        {
            try
            {
                _broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _broadcastSocket.EnableBroadcast = true;

                byte[] broadcastMessage = Encoding.UTF8.GetBytes(
                    _applicationID + "_" +
                    _portNum.ToString() + "_" +
                    _localNetworkInformation.LocalIPAddress.ToString());

                EndPoint localEndPoint = new IPEndPoint(_localNetworkInformation.BroadcastAddress, _portNum);

                ConnnectionStateMessage = "스마트폰에서 Sticker 어플리케이션을 실행해 주세요.";

                while(true)
                {
                    _broadcastSocket.SendTo(broadcastMessage, localEndPoint);
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        ManualResetEvent asyncSocketHandler;

        private void DeviceWaitUsingTCPSocket(object state)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = _localNetworkInformation.LocalIPAddress;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _portNum);

            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            asyncSocketHandler = new ManualResetEvent(false);
            try
            {
                _listenerSocket.Bind(localEndPoint);
                _listenerSocket.Listen(20);

                while(true)
                {
                    asyncSocketHandler.Reset();
                    _listenerSocket.BeginAccept(new AsyncCallback(TCPAcceptCallback), _listenerSocket);
                    asyncSocketHandler.WaitOne();
                }
            }
            catch
            {
                _eventAggregator.GetEvent<NotifyToastEvent>().Publish("TCP 통신에 문제가 발생하여 프로그램을 종료합니다.");

                Thread.Sleep(TimeSpan.FromSeconds(5));
                _eventAggregator.GetEvent<ProgramShutDownEvent>().Publish("network error");
            }
        }

        private void TCPAcceptCallback(IAsyncResult ar)
        {
            asyncSocketHandler.Set();

            Console.WriteLine("TCP AcceptCallback is called");
            Socket connectedDeviceSocket = null;

            try
            {
                connectedDeviceSocket = ((Socket)ar.AsyncState).EndAccept(ar);
            }
            catch
            {
                return;
            }

            _notConfirmSocketList.Add(connectedDeviceSocket);

            // Profile 요청 메시지 전송. 이 메시지에 응답해야 Player로 등록
            byte[] byteData = Encoding.UTF8.GetBytes("Profile_Request\n");
            connectedDeviceSocket.BeginSend(byteData, 0, byteData.Length, 0, null, null);

            // Profile 요청으로부터의 응답 대기
            StateObject state = new StateObject();
            state.socket = connectedDeviceSocket;
            connectedDeviceSocket.BeginReceive(
                    state.buffer,
                    0,
                    StateObject.BufferSize,
                    0,
                    new AsyncCallback(awaitDeviceResponseCallback),
                    state);
        }

        private void awaitDeviceResponseCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.socket;
            int bytesRead = 0;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                _notConfirmSocketList.Remove(handler);
                this.SocketCloser(handler);

                return;
            }

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                string content = state.sb.ToString();
                state.sb.Clear();

                if (content.IndexOf("\n") > -1)
                {
                    content = content.Replace("\n", "");
                    
                    // 해당 문자열이 정상적 응답인 경우
                    if (content.Contains("Profile_Response_"))
                    {
                        GetNewPlayer(handler, content);
                        return;
                    }
                }
            }

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(awaitDeviceResponseCallback), state);
        }

        private void GetNewPlayer(Socket handler, string message)
        {
            string[] tokenize = message.Split('_');

            if (_notConfirmSocketList.Contains(handler))
            {
                _notConfirmSocketList.Remove(handler);
            }

            _getPlayerCallback?.Invoke(new Player(handler, tokenize[2], int.Parse(tokenize[3])));
        }

        private void SocketCloser(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch { } 
            finally
            {
                socket = null;
            }
        }

        private class LocalAddressInformation
        {
            public UnicastIPAddressInformation LocalInfo { get; set; }
            public IPAddress LocalIPAddress { get; set; }
            public IPAddress SubnetMask { get; set; }
            public IPAddress BroadcastAddress { get; set; }

            public LocalAddressInformation()
            {
                LocalInfo = GetLocalUnicastIPAddressInformation();
                if (LocalInfo == null)
                {
                    LocalIPAddress = null;
                    SubnetMask = null;
                    BroadcastAddress = null;
                }
                else
                {
                    LocalIPAddress = LocalInfo.Address;
                    SubnetMask = LocalInfo.IPv4Mask;
                    BroadcastAddress = GetLocalBroadcastAddress();
                }
            }

            private IPAddress GetLocalBroadcastAddress()
            {
                byte[] broadcastIPBytes = new Byte[4];

                for (int i = 0; i < 4; i++)
                {
                    byte inverseSubnetMask = (byte)~(SubnetMask.GetAddressBytes().ElementAt(i));
                    broadcastIPBytes[i] = (byte)(LocalIPAddress.GetAddressBytes().ElementAt(i) | inverseSubnetMask);
                }

                return new IPAddress(broadcastIPBytes);
            }

            private UnicastIPAddressInformation GetLocalUnicastIPAddressInformation()
            {
                // 현재 컴퓨터에 연결된 모든 네트워크 인터페이스를 순회
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // 해당 인터페이스의 게이트웨이 주소를 얻음. 없으면 null.
                    var gatewayAddress = networkInterface.GetIPProperties().GatewayAddresses.FirstOrDefault();

                    // 게이트웨이 주소가 null 이거나 0,0,0,0 인 경우 컴퓨터 내부의 가상 네트워크임. 따라서 제외.
                    if (gatewayAddress == null || gatewayAddress.Address.ToString().Equals("0.0.0.0"))
                    {
                        continue;
                    }

                    // 네트워크 인터페이스 타입이 무선(802.11) 이나 이더넷일 경우에
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        // LINQ 로 무선 or 이더넷 인터페이스의 유니캐스트 주소 중 내부 네트워크인 ip 주소만 추출               
                        IEnumerable<UnicastIPAddressInformation> ip = networkInterface.GetIPProperties().UnicastAddresses
                            .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

                        // 내부 네트워크의 UnicastIPAddressInformation 중에서, 첫번째 옥텟이 10, 172, 192 중 하나인지 확인한다.
                        // 10, 172, 192는 각각 클래스 A, B, C 네트워크의 시작 옥텟으로, 대부분의 공유기가 이 옥텟을 이용해 로컬 네트워크를 할당하기 때문.
                        foreach (UnicastIPAddressInformation item in ip)
                        {
                            if (item.Address.GetAddressBytes()[0].ToString() == "10" || item.Address.GetAddressBytes()[0].ToString() == "172" || item.Address.GetAddressBytes()[0].ToString() == "192")
                            {
                                return ip.FirstOrDefault();
                            }
                        }
                    }
                }
                // 만족하는 결과가 없으면 null 반환
                return null;
            }
        }
    }
}
