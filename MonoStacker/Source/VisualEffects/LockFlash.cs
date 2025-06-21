using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects
{
    public class LockFlash: VisualEffect
    {
        private Rectangle _sourceRect = new Rectangle(8, 8, 8, 8);
        private Texture2D _blocks = GetContent.Load<Texture2D>("Image/Block/0");
        private Vector2 _position;

        protected float diminishTime;
        Color tint;

        public LockFlash(Vector2 position, float timeDisplayed, float diminishTime): base (position)
        {
            _position = position;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
            this.diminishTime = diminishTime;
        }

        public LockFlash(Vector2 position, Color color, float timeDisplayed): base(position)
        {
            _position = position;
            tint = color;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
        }

        public override void Update(float deltaTime)
        {
            TimeDisplayed -= deltaTime;
            tint *= (TimeDisplayed / (MaxTimeDisplayed));
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(_blocks, _position, _sourceRect, tint);
        }
    }
}
