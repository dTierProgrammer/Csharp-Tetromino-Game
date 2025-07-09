using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

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
            EffectManager.Update(gameTime);
            playfield.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
          spriteBatch.Begin();
          spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Background/bg_1080"), new Vector2(0, 0), Color.White);
          spriteBatch.End();
          playfield.Draw(spriteBatch);
          EffectManager.Draw(spriteBatch);
          ParticleManager.Draw(spriteBatch);
        }
    }
}
