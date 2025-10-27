using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class PlayerManager
    {
        private float _speed = 10;
        private float _offset = 20f;
        private readonly Dictionary<string, Player> _players = new(); // luu moi player kem id, de de dang loia khoi danh sach

        public void Profile(string id, float width, float height)
        {
            var player = GetPlayer(id);
            if (player != null)
            {
                player.Width=width;
                player.Height=height;
            }
        }//done

        public void AddPlayer(Player player)
        {
            if (!_players.ContainsKey(player.Id.ToString()))
            {
                _players[player.Id.ToString()]= player;    
            }
        }//done

        public void RemovePlayer(string id)
        {
            if (_players.ContainsKey(id))
            {
                _players.Remove(id);
            }
        }//done

        public Player? GetPlayer(string id)
        {
            _players.TryGetValue(id, out var player);
            return player;
        }//done

        public List<Player> GetAllPlayer()
        {
            return _players.Values.ToList();
        }//done

        public void MovePlayer(string id, string direction)
        {
            var player= GetPlayer(id);
            if (player == null) return;
            if (player.IsDashing) return;
            switch (direction.ToUpper())
            {
                case "UP": player.Y -= _speed; break;
                case "DOWN": player.Y += _speed; break;
                case "LEFT": player.X -= _speed; break;
                case "RIGHT": player.X += _speed; break;
            }
        }//done

        public void RotationPlayer(string id,float rotation)
        {
            var player= GetPlayer(id);
            if (player == null) return;
            player.Rotation=rotation;
        }//done

        public PointF[] BoxCollider(string id, float width, float height)
        {
            var player= GetPlayer(id);
            if (player == null)
            {
                return  Array.Empty<PointF>();
            }

            float centerX= player.X + width/2f - _offset;
            float centerY = player.Y + height / 2f;
            float rad = player.Rotation * (float)(Math.PI / 180f);

            var corners = new[] // toa do 4 goc cua sprite
            {
                new PointF(player.X, player.Y),
                new PointF(player.X+ width, player.Y),
                new PointF(player.X+ width,player.Y+height),
                new PointF(player.X,player.Y+height),
            };

            for (int i = 0; i<corners.Length;i++)
            {
                float dx= corners[i].X - centerX;
                float dy = corners[i].Y - centerY;

                float rotatedX = centerX + dx * MathF.Cos(rad) - dy * MathF.Sin(rad);
                float rotatedY = centerY + dx * MathF.Sin(rad) + dy * MathF.Cos(rad);

                corners[i] = new PointF(rotatedX, rotatedY);
            }

            return corners;
        }//done

        public bool IsPointInPolygon4(PointF[] polygon, PointF testPoint) 
        { 
            bool result = false; 
            int j = polygon.Length - 1; 
            for (int i = 0; i < polygon.Length; i++) 
            { 
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || 
                    polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y) 
                { 
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / 
                        (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < 
                        testPoint.X) 
                    { 
                        result = !result; 
                    } 
                } 
                j = i; 
            } 
            return result; 
        }//done

    }
}
