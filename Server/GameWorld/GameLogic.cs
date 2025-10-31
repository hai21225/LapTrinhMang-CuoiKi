using Server.Game;
using System.Drawing;

namespace Server.GameWorld
{
    public class GameLogic
    {
        private readonly PlayerManager _playerManager;
        private readonly BulletManager _bulletManager;
        private readonly RankManager _rankManager;
        private Random _random = new Random();
        private float _screenWidth = 1280*3;
        private float _screenHeight = 720*3;
        public GameLogic(PlayerManager playerManager, BulletManager bulletManager, RankManager rankManager)
        {
            _playerManager = playerManager;
            _bulletManager = bulletManager;
            _rankManager = rankManager;
        }
        public void Update(float deltaTime)
        {
            HandleCollisions();
            HandleRespawn(deltaTime);
            HandleDashSkill(deltaTime);
            CheckPlayer();
            HandleUltimateSkill(deltaTime);
        }

        private void HandleCollisions()
        {
            var bullets= _bulletManager.GetAllBullets();
            var players = _playerManager.GetAllPlayer();
            foreach(var bullet in bullets.ToList()) 
            {
                bullet.BulletCalculation();// tinh toan di chuyen vien dan
                foreach(var player in players)
                {
                    if (player.Id == bullet.OwnerId) continue;
                    var corners= _playerManager.BoxCollider(player.Id.ToString(),player.Width,player.Height);
                    if (_playerManager.IsPointInPolygon4(corners,new PointF(bullet.X, bullet.Y)))
                    {
                        if (player.Hp >0)
                        {
                            player.Hp -= bullet.Damage;
                            if (player.Hp <= 0)
                            {
                                player.Die();
                                var enemy = _playerManager.GetPlayer(bullet.OwnerId.ToString());
                                if (enemy != null)
                                {
                                   // Console.WriteLine(enemy.Id+" tang diem");
                                    enemy.Score+=10;
                                }
                            }
                            _bulletManager.RemoveBullet(bullet);
                            break;
                        }
                    }
                }
                if (bullet.LifeTimer.Elapsed.TotalSeconds > bullet.TimeExist)
                {
                    _bulletManager.RemoveBullet(bullet);    
                }
            }
        }

        private void HandleRespawn(float deltaTime)
        {
            var players = _playerManager.GetAllPlayer();
            foreach (var player in players)
            {
                if (player.State == PlayerState.Dead && player.Hp<=0)
                {
                    player.RespawnTime -=deltaTime;
                    if (player.RespawnTime < 0)
                    {
                        player.State = PlayerState.Alive;
                        player.Hp = 100;
                        player.X = _random.Next(1, 500);
                        player.Y = _random.Next(1, 500);
                        player.Rotation = _random.Next(-180, 180);
                    }
                }
            }
        }

        private void HandleDashSkill (float deltaTime)
        {
            foreach(var player in _playerManager.GetAllPlayer())
            {
                player.DashSkill(deltaTime);
            }
        }

        private void CheckPlayer()
        {
            foreach(var player in _playerManager.GetAllPlayer())
            {
                if (player.X <= 0 || player.X >= _screenWidth - player.Width)
                    player.X = Math.Clamp(player.X, 0, _screenWidth - player.Width);
                if (player.Y <= 0 || player.Y >= _screenHeight - player.Height)
                    player.Y = Math.Clamp(player.Y, 0, _screenHeight - player.Height);
                //player.X = Math.Max(0, Math.Min(player.X, _screenWidth - player.Width));
                //player.Y = Math.Max(0, Math.Min(player.Y, _screenHeight - player.Height));
            }
        }

        private void HandleUltimateSkill(float deltatime)
        {
            var now = DateTime.Now;
            foreach (var player in _playerManager.GetAllPlayer())
            {
                if (player.UltimateCooldownLeft > 0f)
                {
                    player.UltimateCooldownLeft -= deltatime;
                }

                if (!player.IsUltimateActive) continue;

                if (player.UltimateTimeDurationLeft > 0f)
                {
                    player.UltimateTimeDurationLeft -= deltatime;
                    if (player.UltimateTimeDurationLeft < 0f)
                    {
                        player.IsUltimateActive = false;
                    }
                }
                // Kiểm tra nếu đã đến lúc bắn tiếp theo
                if ((now - player.LastUltimateFireTime).TotalSeconds >= player.UltimateFireRate)
                {
                    int bulletCount = 5;
                    float spreadAngle = 60f;
                    float startAngle = player.Rotation - spreadAngle / 2;

                    for (int i = 0; i < bulletCount; i++)
                    {
                        float angle = startAngle + (spreadAngle / bulletCount) * i;
                        _bulletManager.CreateBullet(angle, player);
                    }

                    player.LastUltimateFireTime = now;
                }
            }
        }


    }
}