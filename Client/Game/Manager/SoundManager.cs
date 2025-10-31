using System;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Game.Manager
{
    public static class SoundManager
    {
        private static SoundPlayer? _background;
        private static CancellationTokenSource? _cts;

        public static void PlayBackground(string path, bool loop = true)
        {
            _background = new SoundPlayer(path);
            _background.Load();
            if (loop)
            {
                _background.PlayLooping();
            }
            else
            {
                _background.Play();
            }
        }

        public static void StopBackground()
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                _background?.Stop();
                _background?.Dispose();
                _background = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop error: " + ex.Message);
            }
        }

        public static void ShootEffect(string path)
        {
            try
            {
                using (var shootEffect = new SoundPlayer(path))
                {
                    shootEffect.Play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shoot sound error: " + ex.Message);
            }
        }
    }
}
