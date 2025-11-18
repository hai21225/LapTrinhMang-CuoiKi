using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DTO
{
    public class ClientAction
    {

        public string Action { get; set; } = string.Empty;
        public string PlayerId { get; set; } = "";

        // các dữ liệu riêng theo action
        public string Direction { get; set; } = "";
        public float Rotation { get; set; }
        public float RotationShoot { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
