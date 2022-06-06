using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Server.Game;

namespace Server.Server
{
    internal class ServerSocket
    {
        private readonly Socket listener;
        private List<Player> players = new List<Player>();
        private Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>(); 
    }
}
