using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float Hp { get; set; }
        public int Ammo { get; set; }

        public bool IsDashing { get; set; }
        public float DashCooldownLeft {  get; set; }
        public float DashCooldown {  get; set; }
        public float UltimateCooldown { get; set; }
        public float UltimateCooldownLeft { get; set; }
        public bool IsUltimateActive { get; set; }

        public PlayerState State { get; set; }


    }

    public enum PlayerState
    {
        Alive,
        Dead
    }
}
