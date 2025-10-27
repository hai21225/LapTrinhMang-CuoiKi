using Client.Game.Input;
using Client.Game.Manager;
using Client.Game.Renderer;
using Client.Models;
using Client.Network;
using Client.Service;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace Client
{
    public partial class Form1 : Form
    {
        private ConnectToServer? _client;
        private PlayerService? _pService;
        private BulletService? _bService;
        private GameLoop? _game;
        private PlayerInputHandler? _input;
        private PlayerRenderer? _playerRenderer;
        private BulletRenderer? _bulletRenderer;



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
            _pService = new PlayerService(_client);
            _bService = new BulletService(_client);
            _input = new PlayerInputHandler(_pService, this, _bService);
            _playerRenderer = new PlayerRenderer(_pService);
            _bulletRenderer=new BulletRenderer(_bService);
            _game = new GameLoop(this);

            _client.ConnectServer();

            _game.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _playerRenderer?.Render(e.Graphics);
            _bulletRenderer?.Render(e.Graphics);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _game?.Stop();
            _client?.DisconnectServer();
        }
    }
}