using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Events
{
    public static class KeyEvent
    {
        public enum KeyType
        {
            UP,
            DOWN,
            RIGHT,
            LEFT,
            SELECT,
            CANCEL
        }

        public enum MusicKeyType
        {
            PLAY,
            STOP,
            NEXT,
            BEFORE,
            LISTUP,
            LISTDOWN,
            REPEAT,
            SHUFFLE
        }
    }

    public class MusicKeYDownEvent : PubSubEvent<KeyEvent.MusicKeyType> { }
    public class MusicControllerKeyEvent : PubSubEvent<String> { }
    public class ReceiveTextDataEvent : PubSubEvent<string> { }

    public class MasterKeyDownEvent : PubSubEvent<KeyEvent.KeyType> { }


    public class MasterConnectionLossEvent : PubSubEvent<string> { }
}

