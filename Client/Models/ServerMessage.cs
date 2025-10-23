using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ServerMessage
    {
        public string Action { get; set; } = string.Empty;
        public List<Player>? Players { get; set; }
        public List<Bullet>? Bullets { get; set; }
    }
}
