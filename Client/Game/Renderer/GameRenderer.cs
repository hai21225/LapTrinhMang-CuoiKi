using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class GameRenderer
    {
        private readonly PlayerRenderer _playerRenderer;
        private readonly BulletRenderer _bulletRenderer;
        private readonly Camera _camera;
        private  Image _background= Properties.Resources.background;

        private readonly int _worldWidth = 3840;
        private readonly int _worldHeight = 2160;

        public GameRenderer(PlayerRenderer playerRenderer, BulletRenderer bulletRenderer, Camera camera)
        {
            _playerRenderer = playerRenderer;
            _bulletRenderer = bulletRenderer;
            _camera = camera;
        }

        public void Render(Graphics g)
        {
            var bgPos = _camera.WorldToScreen(0, 0);
            g.DrawImage(_background, bgPos.X, bgPos.Y, _worldWidth, _worldHeight);

            _playerRenderer.Render(g);
            _bulletRenderer.Render(g);
            _playerRenderer.RenderPlayerDead(g);
        }
    }
}
