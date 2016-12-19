package com.cro.stickerapp.global;

import android.os.AsyncTask;
import android.support.annotation.NonNull;
import android.util.Log;

import com.cro.stickerapp.WaitDeviceActivity;
import com.cro.stickerapp.baseModule.BaseControllerActivity;
import com.cro.stickerapp.classes.Profile;
import com.cro.stickerapp.musicModule.MusicControllerActivity;
import com.cro.stickerapp.oneCardModule.OneCardControllerActivity;
import com.cro.stickerapp.oneCardModule.OneCardWaitActivity;
import com.cro.stickerapp.rpsModule.RPSControllerActivity;
import com.cro.stickerapp.rpsModule.RPSWaitActivity;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.URLEncoder;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class GlobalNetworkService {
    //region Singleton
    private static GlobalNetworkService _instance = new GlobalNetworkService();

    public static GlobalNetworkService getInstance() {
        return _instance;
    }

    private GlobalNetworkService() {
    }
    //endregion

    //region Property

    private static final int _port = 19258;
    private String _ipAddress;

    //endregion

    public interface NetworkConnectionCallback<T>{
        void connectionFinished(T result);
    }

    //region UDP Connection

    public void initUDPBroadcastMessageReceiver(NetworkConnectionCallback<String> udpCallback) {
        UDPBroadcastMessageReceiver receiver = new UDPBroadcastMessageReceiver(udpCallback);
        receiver.execute();
    }

    // AsyncTask parameter sequence is param, progress, and result
    private class UDPBroadcastMessageReceiver extends AsyncTask<Void, Void, String> {
        NetworkConnectionCallback<String> _udpCallback;

        public UDPBroadcastMessageReceiver(NetworkConnectionCallback<String> udpCallback) {
            this._udpCallback = udpCallback;
        }

        @Override
        protected String doInBackground(Void... voids) {
            DatagramSocket socket = null;
            String message = null;

            try {
                socket = new DatagramSocket(_port);

                // 60000 ms = 60 * 1000 = 1min
                socket.setSoTimeout(60000);

                while (true) {
                    byte[] buf = new byte[1024];
                    DatagramPacket packet = new DatagramPacket(buf, buf.length);
                    socket.receive(packet);

                    String msg = new String(packet.getData(), 0, packet.getLength());

                    Log.d("testMessage", msg);

                    if (msg.contains("Sticker") && msg.contains("19258")) {
                        String[] tokenize = msg.split("_");

                        _ipAddress = tokenize[2];
                        message = msg;
                        break;
                    }
                    Thread.sleep(500);
                }
            } catch (Exception e) {
                message = null;
                Log.d("GlobalNetworkService", "Exception : Can't receive host's UDP message.");
            } finally {
                if (socket != null) {
                    socket.disconnect();
                    socket.close();
                }
            }
            return message;
        }

        @Override
        protected void onPostExecute(String Result) {
            super.onPostExecute(Result);

            if (this._udpCallback != null) {
                this._udpCallback.connectionFinished(Result);
            }
        }
    }

    //endregion

    //region TCP Connection

    private TCPClient _client = null;
    private NetworkConnectionCallback<Boolean> _tcpCallback = null;

    public void initTCPConnection(NetworkConnectionCallback<Boolean> tcpCallback) {
        _tcpCallback = tcpCallback;

        _client = new TCPClient();
        _client.setReceiveCallback(new NetworkReceiver());
        _client.connectionStart(_ipAddress, _port);
    }

    public class TCPClient extends Thread
    {
        private Socket _socket;
        private BufferedReader _reader;
        private PrintWriter _writer;

        private NetworkConnectionCallback<String> _receiveCallback;

        private NetworkReceiveThread _receiveThread = new NetworkReceiveThread();

        private String _ipAddress;
        private int _portNum;

        public void setReceiveCallback(NetworkConnectionCallback<String> _receiveCallback) {
            this._receiveCallback = _receiveCallback;
        }

        public void connectionStart(String ipAddress, int portNum)
        {
            _ipAddress = ipAddress;
            _portNum = portNum;

            this.start();
        }

        @Override
        public void run() {
            try{
                InetSocketAddress socketAddress = new InetSocketAddress(InetAddress.getByName(_ipAddress), _portNum);
                _socket = new Socket();
                _socket.connect(socketAddress, 10000);

                _reader = new BufferedReader(new InputStreamReader(_socket.getInputStream()));
                _writer = new PrintWriter(new BufferedWriter(new OutputStreamWriter(_socket.getOutputStream())), true);

                _receiveThread.start();
                Log.d("NetworkService", "Setting clear");
            }catch (Exception e)
            {
                this.closeConnect();
                _tcpCallback.connectionFinished(false);
            }
        }

        public void sendMessageToServer(String message)
        {
            try {
                if(_writer!=null) {
                    Log.d("SendMessage", message);
                    _writer.println(message);
                }
            }catch (Exception e)
            {
                Log.d("GlobalNetworkService", "Fail send message to server");
            }
        }

        private class NetworkReceiveThread extends Thread
        {
            @Override
            public void run() {
                try{
                    while(true)
                    {
                        String receive = _reader.readLine();

                        if(receive!=null && _receiveCallback!=null)
                        {
                            _receiveCallback.connectionFinished(receive);
                        }

                        if(receive==null) throw new Exception();
                    }
                }catch (Exception e)
                {
                    GlobalEngine.getInstance().finishApp("서버와의 연결이 끊어졌습니다.");
                    //closeConnect();
                }
            }
        }

        public void closeConnect() {
            try{
                if(_socket != null) {
                    _socket.close();

                    _receiveCallback = null;
                    _reader = null;
                    _writer = null;
                }
            }catch (Exception e)
            {
                // do nothing
            }

        }
    }
    //endregion

    private boolean _isConnectionSuccess = false;

    public boolean isConnectionSuccess() {
        return _isConnectionSuccess;
    }

    public class NetworkReceiver implements NetworkConnectionCallback<String> {
        @Override
        public void connectionFinished(final String result) {
            Log.d("NetworkService", "Receive Message - " + result);
            String[] tokenize = result.split("_");
            switch (tokenize[0])
            {
                case "Confirm":
                {
                    return;
                }
                //region Profile
                case "Profile":
                {
                    if(tokenize[1].equals("Request"))
                    {
                        Profile profile = GlobalEngine.getInstance().get_profile();

                        GlobalNetworkService.getInstance().sendMessageToServer("Profile_Response_" + profile.get_playerName() + "_" + profile.get_profileImageNum());

                        if(_tcpCallback != null)
                        {
                            _isConnectionSuccess = true;
                            _tcpCallback.connectionFinished(true);
                        }
                    }else if(tokenize[1].equals("KeyboardRequest"))
                    {
                        GlobalService.getInstance().requestKeyboardService();
                    }else if(tokenize[1].equals("Save"))
                    {
                        Profile profile = GlobalEngine.getInstance().get_profile();
                        profile.makeProfile(tokenize[2], Integer.parseInt(tokenize[3]));
                    }
                }break;
                //endregion

                //region Request
                case "Request":
                {
                    if(tokenize[1].equals("Navigate"))
                    {
                        switch (tokenize[2])
                        {
                            case "Base":
                            {
                                GlobalEngine.getInstance().requestNavigate(BaseControllerActivity.class, 0);
                            }break;
                            case "WaitDevice":
                            {
                                GlobalEngine.getInstance().requestNavigate(WaitDeviceActivity.class, 0);
                            }break;
                            case "RPSWait":
                            {
                                GlobalEngine.getInstance().requestNavigate(RPSWaitActivity.class, 0);
                            }break;
                            case "RPSController":
                            {
                                GlobalEngine.getInstance().requestNavigate(RPSControllerActivity.class, 0);
                            }break;
                            case "Music":
                            {
                                GlobalEngine.getInstance().requestNavigate(MusicControllerActivity.class, 0);
                            }break;
                            case "OneCardWait":
                            {
                                GlobalEngine.getInstance().requestNavigate(OneCardWaitActivity.class, 0);
                            }break;
                            case "OneCardController":
                            {
                                GlobalEngine.getInstance().requestNavigate(OneCardControllerActivity.class, 0);
                            }break;
                            default:
                                break;
                        }
                    }
                }break;
                //endregion
                case "Sticker":
                default:
                {
                    GlobalEngine.getInstance().sendMessageToActivity(result);
                }break;
            }
        }
    }

    public void sendMessageToServer(String message)
    {
        if(_client!=null)
        {
            _client.sendMessageToServer(message);
        }
    }

    public TCPClient getTCPClient()
    {
        return _client;
    }
}