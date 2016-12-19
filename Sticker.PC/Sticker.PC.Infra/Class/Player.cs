//using Prism.Mvvm;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace Sticker.PC.Infra.Class
//{
//    public class Player : BindableBase, IDisposable
//    {
//        #region IDisposable Support
//        private bool disposedValue = false; // 중복 호출을 검색하려면

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                }

//                try
//                {
//                    Socket.Shutdown(SocketShutdown.Both);
//                    Socket.Close();
//                }
//                catch (Exception)
//                {
//                    // do nothing
//                }
//                finally
//                {
//                    Socket = null;
//                }

//                disposedValue = true;
//            }
//        }

//         ~Player()
//        {
//            Dispose(false);
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//        #endregion

//        public enum PlayerType
//        {
//            Master,
//            Player
//        }

//        public Socket Socket { get; private set; }
//        public PlayerType Type { get; set; }


//        private string _nickName;
//        public string Nickname
//        {
//            get { return _nickName; }
//            set { SetProperty(ref _nickName, value); }
//        }

//        private int _thumbnailNum;
//        public int ThumbnailNum
//        {
//            get { return _thumbnailNum; }
//            set { SetProperty(ref _thumbnailNum, value); }
//        }

//        public string ThumbnailPath
//        {
//            get
//            {
//                return "/Sticker.PC.Infra;component/Resources/Images/Profile/Avatar/img_thumb" + ThumbnailNum.ToString() + ".png";
//            }
//        }

//        public string ShadowThumbnailPath
//        {
//            get
//            {
//                return "/Sticker.PC.Infra;component/Resources/Images/Profile/Avatar/img_thumb" + ThumbnailNum.ToString() + "_shadow.png";
//            }
//        }

//        public Player(Socket socket, string nickname, int thumbnailNum, PlayerType type = PlayerType.Player)
//        {
//            this.Socket = socket;
//            this.Nickname = nickname;
//            this.ThumbnailNum = thumbnailNum;
//            this.Type = type;
//        }
//    }
//}

using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Class
{
    public class Player : BindableBase, IDisposable
    {
        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
                catch (Exception)
                {
                    // do nothing
                }
                finally
                {
                    Socket = null;
                }

                disposedValue = true;
            }
        }

        ~Player()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public enum PlayerType
        {
            Master,
            Player
        }

        public Socket Socket { get; private set; }
        public PlayerType Type { get; set; }


        private string _nickName;
        String key = "key";

        public string Nickname
        {
            get { return _nickName; }
            set { SetProperty(ref _nickName, value); }
        }

        private int _thumbnailNum;
        public int ThumbnailNum
        {
            get { return _thumbnailNum; }
            set { SetProperty(ref _thumbnailNum, value); }
        }

        public string ThumbnailPath
        {
            get
            {
                return "/Sticker.PC.Infra;component/Resources/Images/Profile/Avatar/img_thumb" + ThumbnailNum.ToString() + ".png";
            }
        }

        public string ShadowThumbnailPath
        {
            get
            {
                return "/Sticker.PC.Infra;component/Resources/Images/Profile/Avatar/img_thumb" + ThumbnailNum.ToString() + "_shadow.png";
            }
        }

        public static string Decrypt(string textToDecrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public Player(Socket socket, string nickname, int thumbnailNum, PlayerType type = PlayerType.Player)
        {
            this.Socket = socket;
        //    string de = Decrypt(nickname, key);
            this.Nickname = nickname;
            this.ThumbnailNum = thumbnailNum;
            this.Type = type;

            //Console.WriteLine("Encrypted Text is " + nickname);
            //Console.WriteLine("Decrypted Text is " + de);
            //Console.WriteLine("So the Nickname is " + Nickname);
        }
    }
}