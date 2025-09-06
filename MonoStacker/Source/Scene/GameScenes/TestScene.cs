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
using RasterFontLibrary.Source;
using MonoStacker.Source.Data;
using MonoStacker.Source.Interface.Input;

namespace MonoStacker.Source.Scene.GameScenes
{
    public class TestScene: IScene
    {
        PlayField playfield;
        private RasterFont _rasterFont;

        public void Initialize() 
        {
            playfield = new PlayField(new Vector2(240, 135), PlayFieldPresets.Arcade3, new InputBinds() );
            playfield.Start();
        }
        public void Load() 
        {
            _rasterFont = new(GetContent.Load<Texture2D>("Image/Font/small_outline"), 1, 1, -1);
        }

        public void Update(GameTime gameTime) 
        {
            playfield.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
          spriteBatch.Begin();
          //spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Background/bg_1080"), new Vector2(0, 0), Color.White);
           
          spriteBatch.End();
          playfield.Draw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, null);
          _rasterFont.RenderString(spriteBatch, Vector2.One, "for debug purposes", Color.Yellow);
            spriteBatch.End();
        }

        public void DrawText(SpriteBatch spriteBatch) { }
    }
}
