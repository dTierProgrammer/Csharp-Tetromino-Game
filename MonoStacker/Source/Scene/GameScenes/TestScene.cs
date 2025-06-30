using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Scene.GameScenes
{
    public class TestScene: IScene
    {
        PlayField playfield;
        public void Initialize() 
        {
            playfield = new PlayField(new Vector2(86, 47));
            playfield.Initialize();
        }

        public void Load() { }

        public void Update(GameTime gameTime) 
        {
            playfield.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            playfield.Draw(spriteBatch);
        }
    }
}
