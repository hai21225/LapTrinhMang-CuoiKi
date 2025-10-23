using Client.Logic;
using Client.Network;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cts;

        private ConnectToServer _client;
        private ClientGameService _service;
        private HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer _moveTimer;
        private Image _playerImage = Properties.Resources.right;
        private float _rotation = 0f;
        private string? _direction = "";
        private float _pivotX= 0f;
        private float _pivotY= 0f;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _client = new ConnectToServer();
            _service = new ClientGameService(_client);

            _service.OnPlayerUpdated += players => { };
            _service.OnBulletUpdated += bullets => { };
            _service.OnInitCompleted += () =>
            {
                _service.SendProfile(_playerImage.Width, _playerImage.Height);
            };
            _client.ConnectServer();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            _moveTimer = new System.Windows.Forms.Timer();
            _moveTimer.Interval = 50; // 20/s
            _moveTimer.Tick += MoveTimer_Tick;
            _moveTimer.Start();
        }

        private async void StartGameLoop()
        {
            _cts = new CancellationTokenSource();
            var sw = new Stopwatch();
            const int frameTime = 16; // 16ms ~ 60 FPS

            while (!_cts.Token.IsCancellationRequested)
            {
                sw.Restart();
                this.Invalidate();

                var elapsed = (int)sw.ElapsedMilliseconds;
                int delay = frameTime - elapsed;
                if (delay > 0)
                    await Task.Delay(delay);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartGameLoop();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.KeyCode);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }
        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (_service.GetMyId == null) return;

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
                    //Console.WriteLine(direction);
                    _service.SendMove(_direction);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var p in _service.GetPlayers())
            {
                float imageWidth = _playerImage.Width;
                float imageHeight = _playerImage.Height;

                float offsetX = imageWidth / 2f - 20f;
                float offsetY = imageHeight / 2f;

                float centerX = p.X + offsetX;
                float centerY = p.Y + offsetY;

                var matrix = g.Transform;

                g.TranslateTransform(centerX, centerY);
                g.RotateTransform(p.Rotation);
                g.TranslateTransform(-centerX, -centerY);

                // sprite player
                g.DrawImage(_playerImage, p.X, p.Y, imageWidth, imageHeight);

                // Vẽ viền hình chữ nhật quanh player (debug)
                g.DrawRectangle(Pens.Red, p.X, p.Y, imageWidth, imageHeight);
                // Vẽ tâm xoay
                g.FillEllipse(Brushes.Blue, centerX - 2, centerY - 2, 4, 4);
                // Vẽ HP
                g.DrawString($"HP: {p.Hp}", this.Font, Brushes.White, p.X + imageWidth + 5, p.Y - 20);

                g.Transform = matrix;
            }
            foreach (var b in _service.GetBullets())
            {
                float bulletSize = 10f;
                float bulletX = b.X - bulletSize / 2;
                float bulletY = b.Y - bulletSize / 2;

                g.FillEllipse(Brushes.Yellow, bulletX, bulletY, bulletSize, bulletSize);
                //  (debug)
                //g.DrawEllipse(Pens.Red, bulletX, bulletY, bulletSize, bulletSize);
            }
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            var me = _service.GetPlayers().FirstOrDefault(p => p.Id.ToString() == _service.GetMyId);
            if (me == null) return;

            _pivotX = (me.X + _playerImage.Width / 2 - 20f);
            _pivotY= (me.Y + _playerImage.Height / 2);

            float dx = e.X - (me.X + _playerImage.Width/2-20f);
            float dy = e.Y - (me.Y + _playerImage.Height/2);

            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            _service.SendRotation(_rotation);
        }
        private void MoveButtunDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var me = _service.GetPlayers().FirstOrDefault(p => p.Id.ToString() == _service.GetMyId);
                if (me != null)
                {
                    Console.WriteLine(me.X); Console.WriteLine(me.Y);

                    Console.WriteLine(me.X + _playerImage.Width / 2-20f);
                    Console.WriteLine(me.Y + _playerImage.Height / 2);
                    //Console.WriteLine("checkk");
                    //Console.WriteLine(_rotation + "   " + me.Rotation);
                    _service.SendShoot(_rotation,_pivotX,_pivotY);
                }
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cts?.Cancel();
            _moveTimer?.Stop();
            _client.DisconnectServer();
        }

    }
}