using System.Diagnostics;


namespace Client.Game.Manager
{
    public class GameLoop
    {
        private readonly Form _form;
        private CancellationTokenSource? _cts;
        private Task? _loopTask;

        public GameLoop(Form form)
        {
            _form = form;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _loopTask =Task.Run(() => LoopAsync(_cts.Token));
        }
        public async Task LoopAsync (CancellationToken token)
        {
            const int frameTime = 24;
            var sw = new Stopwatch();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    sw.Restart();

                    if (!_form.IsDisposed && _form.Created)
                        _form.Invalidate();

                    int delay = frameTime - (int)sw.ElapsedMilliseconds;
                    if (delay > 0)
                        await Task.Delay(delay, token); // truyền token vô luôn
                }
            }
            catch (TaskCanceledException)
            {
                // ignore: task stopped bình thường
            }
            catch (ObjectDisposedException)
            {
                // form bị dispose trong khi loop chạy
            }
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                try
                {
                    _loopTask?.Wait(1000); // chờ 1s cho task dừng hẳn
                }
                catch { }
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}
