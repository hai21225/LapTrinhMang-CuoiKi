using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class PlayerManager
    {
        private int _speed = 2;
        private readonly Dictionary<string, Player> _players = new();

        public void AddPlayer(Player player)
        {
            if (!_players.ContainsKey(player.Id.ToString()))
            {
                _players[player.Id.ToString()]= player;    
            }
        }
        public void RemovePlayer(string id)
        {
            if (_players.ContainsKey(id))
            {
                _players.Remove(id);
            }
        }
        public Player? GetPlayer(string id)
        {
            _players.TryGetValue(id, out var player);
            return player;
        }

        public List<Player> GetAllPlayer()
        {
            return _players.Values.ToList();
        }

        public void MovePlayer(string id, string direction)
        {
            var player= GetPlayer(id);
            if (player == null) return;
            switch (direction.ToUpper())
            {
                case "UP": player.Y -= _speed; break;
                case "DOWN": player.Y += _speed; break;
                case "LEFT": player.X -= _speed; break;
                case "RIGHT": player.X += _speed; break;
            }
        }
        public void RotationPlayer(string id,float rotation)
        {
            var player= GetPlayer(id);
            if (player == null) return;
            player.Rotation=rotation;
        }
    }
}
