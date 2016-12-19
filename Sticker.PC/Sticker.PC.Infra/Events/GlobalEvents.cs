using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Events
{
    public class ProgramShutDownEvent : PubSubEvent<object> { }

    public class NotifyToastEvent : PubSubEvent<string> { }
    public class NetworkStatusNotifyEvent : PubSubEvent<String> { }
}
