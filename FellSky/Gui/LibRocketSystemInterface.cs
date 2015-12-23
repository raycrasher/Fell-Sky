using FellSky.Framework;
using System;

namespace FellSky.Gui
{
    class LibRocketSystemInterface : LibRocketNet.SystemInterface
    {
        private ITimerService _timer;

        public LibRocketSystemInterface(ITimerService timer)
        {
            _timer = timer;
        }
        protected override float GetElapsedTime()
        {
            return (float) _timer.LastFrameUpdateTime.ElapsedGameTime.TotalSeconds;
        }

        protected override bool LogMessage(LibRocketNet.LogType type, string message)
        {
            Console.WriteLine("GUI: {0}", message);
            return true;
        }

        protected override void JoinPath(ref string translatedPath, string documentPath, string path)
        {
            Console.WriteLine("{0} {1}", documentPath, path);
            base.JoinPath(ref translatedPath, documentPath, path);
        }
    }}
