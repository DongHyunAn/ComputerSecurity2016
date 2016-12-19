using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sticker.PC.Module.App.Radio.Class
{
    class RadioChannel
    {
        public string ChannelName { get; set; }

        public string ChannelUri { get; set; }

        [XmlIgnore]
        public Uri ChannelStreamUri => new Uri(ChannelUri);
    }
}
