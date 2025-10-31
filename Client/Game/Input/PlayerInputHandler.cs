using Client.Game.Manager;
using Client.Models;
using Client.Service;
using Timer = System.Windows.Forms.Timer;

namespace Client.Game.Input
{
    public class PlayerInputHandler
    {
        private readonly PlayerService _client;
        private readonly BulletService _bullet;
        private readonly Camera _camera;
        private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private readonly Form _form;
        private Player? _me=null;
        private Image _playerImage = Properties.Resources.right;
        private float _rotation;
        private string? _direction = "";
        private Timer _timer= new Timer();
        private bool _isChatting = false;
        private string _chatInput = string.Empty;

        public PlayerInputHandler(PlayerService client, Form form, BulletService bullet, Camera camera)
        {
            _client = client;
            _bullet = bullet;
            _camera = camera;
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
            _form.KeyPress += OnKeyPress!;

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
            if (!_isChatting)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    _isChatting = true; // bật chế độ chat
                    return;
                }

                _pressedKeys.Add(e.KeyCode);

                if (e.KeyCode == Keys.Q) _client.SendGunReload();
                if (e.KeyCode == Keys.E) _client.SendDash();
                if (e.KeyCode == Keys.Space) _client.SendUltimate();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_isChatting) return;

            if (e.KeyChar == (char)13) // Enter
            {
                if (!string.IsNullOrWhiteSpace(_chatInput))
                    _client.SendChat(_chatInput);

                _chatInput = string.Empty;
                _isChatting = false;
            }
            else if (e.KeyChar == (char)27) // Escape
            {
                _chatInput = string.Empty;
                _isChatting = false;
            }
            else if (e.KeyChar == '\b' && _chatInput.Length > 0)
            {
                _chatInput = _chatInput[..^1];
            }
            else if (!char.IsControl(e.KeyChar))
            {
                _chatInput += e.KeyChar;
            }

            e.Handled = true;
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_me == null || _me.State == PlayerState.Dead || _me.IsDashing|| _me.IsUltimateActive) return;

            var worldMouse = _camera.ScreenToWorld(e.X, e.Y);


            float dx = worldMouse.X - (_me.X + _playerImage.Width / 2 - 20f);
            float dy = worldMouse.Y - (_me.Y + _playerImage.Height / 2);

            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            _client.SendRotation(_rotation);

        }

        private void OnMosueDown(object sender,MouseEventArgs e)
        {
            if (_me == null || _me.State==PlayerState.Dead||_me.IsUltimateActive||_me.IsDashing || _me.Ammo==0) return;
            if (e.Button == MouseButtons.Left)
            {
                _bullet.SendShoot(_me.Id.ToString(), _rotation);
                //SoundManager.ShootEffect("D:\\LapTrinhMang\\CuoiKi\\GameHayNhatTheGioi\\Client\\Sounds\\GunShot.wav");
            }
        }

        public bool IsChatting => _isChatting;
        public string ChatInput => _chatInput;
    }
}
