using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
   public class ChatClient
    {
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime  Time { get; set; } =DateTime.Now;
    }
}
