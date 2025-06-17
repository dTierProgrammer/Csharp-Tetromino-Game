using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Global
{
    public static class GetContent
    {
        private static Game _game;

        public static void Initialize(Game game) 
        {
            _game = game;
        }

        public static T Load<T>(string path)
        {
            return _game.Content.Load<T>(path);
        }
    }
}
