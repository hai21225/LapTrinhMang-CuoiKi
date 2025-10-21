using Client.DTO;
using Client.Logic;
using System.Text.Json;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private ConnectToServer _client;
        private ClientGameService _service;
        private HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer _moveTimer;
        private Image _playerImage = Properties.Resources.right;
        private float _rotation = 0f;
        private string? _direction = "";

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 16; // 60 FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _client = new ConnectToServer();
            _service = new ClientGameService(_client);

            _service.OnPlayerUpdated += players => this.Invalidate();
            _client.ConnectServer();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            _moveTimer = new System.Windows.Forms.Timer();
            _moveTimer.Interval = 50; // 20/s
            _moveTimer.Tick += MoveTimer_Tick;
            _moveTimer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _client.DisconnectServer();
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
                float centerX = p.X + 16;
                float centerY = p.Y + 16;

                // Lưu ma trận gốc
                var matrix = g.Transform;

                // Dịch đến tâm player rồi xoay
                g.TranslateTransform(centerX, centerY);
                g.RotateTransform(p.Rotation);  // hoặc _rotationAngle nếu là local player
                g.TranslateTransform(-centerX, -centerY);

                // Vẽ player
                g.DrawImage(_playerImage, p.X, p.Y, 32, 32);
                g.DrawString($"ID: {p.Id}", this.Font, Brushes.White, p.X, p.Y - 15);

                // Trả ma trận về gốc
                g.Transform = matrix;
            }
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            var me = _service.GetPlayers().FirstOrDefault(p => p.Id.ToString() == _service.GetMyId);
            if (me == null) return;

            float dx = e.X - (me.X + 16);
            float dy = e.Y - (me.Y + 16);

            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            _service.SendRotation(_rotation);
            //this.Invalidate();
        }
    }
}