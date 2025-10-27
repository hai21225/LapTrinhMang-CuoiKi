using Client.Models;
using Client.Service;
using Timer = System.Windows.Forms.Timer;

namespace Client.Game.Input
{
    public class PlayerInputHandler
    {
        private readonly PlayerService _client;
        private readonly BulletService _bullet;
        private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private readonly Form _form;
        private Player? _me=null;
        private Image _playerImage = Properties.Resources.right;
        private float _rotation;
        private string? _direction = "";
        private Timer _timer= new Timer();

        public PlayerInputHandler(PlayerService client, Form form, BulletService bullet)
        {
            _client = client;
            _bullet = bullet;
            _form = form;
            _client.OnGetMyPlayer += me =>
            {
                _me = me;
            };
            _form.KeyPreview = true;

            _form.KeyDown += OnkeyDown!;
            _form.KeyUp += OnKeyUp!;
            _form.MouseMove += OnMouseMove!;
            _form.MouseDown += OnMosueDown!;


            _timer = new Timer();
            _timer.Interval = 50; // 20/s
            _timer.Tick += HandleMove;
            _timer.Start();

        }

        private void HandleMove(object? sender, EventArgs e)
        {
            if (_me == null || _me.State == PlayerState.Dead || _me.IsUltimateActive ||_me.IsDashing) return;
            foreach (var key in _pressedKeys)
            {
                _direction = key switch
                {
                    Keys.W => "UP",
                    Keys.S => "DOWN",
                    Keys.A => "LEFT",
                    Keys.D => "RIGHT",
                    _ => null
                };

                if (_direction != null)
                {
                    _client.SendMove(_direction);
                }
            }
        }

        private void OnkeyDown(object sender, KeyEventArgs e)
        {
            if(_me == null) { return; }

            _pressedKeys.Add(e.KeyCode);

            if (e.KeyCode == Keys.Q)
            {
                _client.SendGunReload();
            }
            if (e.KeyCode == Keys.E)
            {
                _client.SendDash();
            }
            if (e.KeyCode == Keys.Space)
            {
                _client.SendUltimate();
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_me == null || _me.State == PlayerState.Dead || _me.IsDashing|| _me.IsUltimateActive) return;

            float dx = e.X - (_me.X + _playerImage.Width / 2 - 20f);
            float dy = e.Y - (_me.Y + _playerImage.Height / 2);

            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            _client.SendRotation(_rotation);

        }

        private void OnMosueDown(object sender,MouseEventArgs e)
        {
            if (_me == null || _me.State==PlayerState.Dead||_me.IsUltimateActive||_me.IsDashing) return;
            if (e.Button == MouseButtons.Left)
            {
                _bullet.SendShoot(_me.Id.ToString(), _rotation);
            }
        }
    }
}
