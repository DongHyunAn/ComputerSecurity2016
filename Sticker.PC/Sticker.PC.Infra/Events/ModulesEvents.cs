using Prism.Events;
using Sticker.PC.Infra.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticker.PC.Infra.Events
{
    public class CommandFromPlayerParam
    {
        public string Message { get; set; }
        public Player Sender { get; set; }
    }

    public class CommandFromPlayerEvent : PubSubEvent<CommandFromPlayerParam> { }
}
