using Sticker.PC.Infra.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Sticker.PC.Infra.Service.NetworkService.NetworkService;

namespace Sticker.PC.Infra.Service.NetworkService
{
    public interface INetworkService
    {
        void SendMessageToPlayer(Player player, string message);
        void SetAppController(Player player, ControllerType controller);

        Player GetMasterPlayer();
        ObservableCollection<Player> GetPlayers();
    }
}
