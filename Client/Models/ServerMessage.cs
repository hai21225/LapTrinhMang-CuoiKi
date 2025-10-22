using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.DTO
{
    public class ServerMessage
    {
        public string Action { get; set; } = string.Empty;
        public List<PlayerDTO>? Players { get; set; }
        public List<Bullet>? Bullets { get; set; }
    }
}
