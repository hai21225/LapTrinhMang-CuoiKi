using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Manager
{
    public class GameLoop
    {
        private readonly Form _form;
        private CancellationTokenSource? _cts;

        public GameLoop(Form form)
        {
            _form = form;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => LoopAsync(_cts.Token));
        }
        public async Task LoopAsync (CancellationToken token)
        {
            const int frameTime = 24;
            var sw = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                sw.Restart();
                _form.Invalidate();

                int delay = frameTime - (int)sw.ElapsedMilliseconds;
                if (delay > 0)
                    await Task.Delay(delay);
            }
        }
        public void Stop() => _cts?.Cancel();
    }
}
