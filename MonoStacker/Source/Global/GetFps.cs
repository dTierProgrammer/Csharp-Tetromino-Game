using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Global
{
    public static class GetFps
    {
        public static double fps { get; private set; }
        private static int _frames;
        private static double _time;

        public static void Update(GameTime gameTime) 
        {
            _time += gameTime.ElapsedGameTime.TotalSeconds;
            _frames++;

            if (_time >= 1) 
            {
                fps = _frames / _time;
                _time = 0;
                _frames = 0;
            }
        }
    }
}
