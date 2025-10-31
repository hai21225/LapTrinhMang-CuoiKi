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
        private string _playerName;
        private ConnectToServer? _client;
        private PlayerService? _pService;
        private BulletService? _bService;
        private RankService? _rankService;
        private ChatService? _chatService;
        private GameLoop? _game;
        private PlayerInputHandler? _input;
        private GameRenderer? _render;
        private GameUI? _gameUI;
        private PlayerRenderer? _playerRenderer;
        private BulletRenderer? _bulletRenderer;
        private SkillRenderer? _skillRenderer;
        private RankRenderer? _rankRenderer;
        private ChatRenderer? _chatRenderer;
        private Camera? _camera;



        public Form1(string playerName)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            _playerName = playerName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _client = new ConnectToServer();
            _pService = new PlayerService(_client,_playerName);
            _bService = new BulletService(_client);
            _rankService = new RankService(_client);
            _chatService= new ChatService(_client);
            _camera = new Camera(this.ClientSize.Width, this.ClientSize.Height);
            _input = new PlayerInputHandler(_pService, this, _bService,_camera);
            _playerRenderer = new PlayerRenderer(_pService,_camera);
            _bulletRenderer=new BulletRenderer(_bService,_camera);
            _render=new GameRenderer(_playerRenderer, _bulletRenderer,_camera);
            _skillRenderer = new SkillRenderer(_pService);
            _rankRenderer=new RankRenderer(_rankService);
            _chatRenderer = new ChatRenderer(_chatService);
            _gameUI = new GameUI(_input, _rankRenderer, _skillRenderer, _chatRenderer);
            _game = new GameLoop(this);

            _client.ConnectServer();
            //SoundManager.PlayBackground("*/Sounds/Boss.wav");
            _game.Start();
            Console.WriteLine(_playerName);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _render?.Render(e.Graphics);
            _gameUI?.Render(e.Graphics);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _game?.Stop();
            SoundManager.StopBackground();
            _client?.DisconnectServer();
            Application.Exit();
        }
    }
}