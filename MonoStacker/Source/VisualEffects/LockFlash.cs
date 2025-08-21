using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects
{
    public class LockFlash: AnimatedEffect 
    {
        private Rectangle? _sourceRect = null;
        private Texture2D _blocks = GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect");
        private Vector2 _position;
        private float rectWidth = 8;
        private float rectHeight = 8;
        protected float diminishTime;
        protected Vector2 distortFactor = Vector2.Zero;
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

        public LockFlash(Vector2 position, Color color, float timeDisplayed, Vector2 distortFactor) : base(position)
        {
            _position = position;
            tint = color;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
            this.distortFactor = distortFactor;
        }

        public LockFlash(Texture2D image, Vector2 position, Color color, float timeDisplayed, Vector2 distortFactor) : base(position)
        {
            _blocks = image;
            _position = position;
            tint = color;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
            this.distortFactor = distortFactor;
        }

        public LockFlash(Texture2D image, Rectangle sourceRect, Vector2 position, Color color, float timeDisplayed, Vector2 distortFactor) : base(position)
        {
            _blocks = image;
            _position = position;
            tint = color;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
            this.distortFactor = distortFactor;
            _sourceRect = sourceRect;
        }

        public override void Update(float deltaTime)
        {
            TimeDisplayed -= deltaTime;
            tint *= (TimeDisplayed / (MaxTimeDisplayed));
            rectWidth += distortFactor.X;
            rectHeight += distortFactor.Y;
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw
                (_blocks,
                new Rectangle((int)_position.X + 4, (int)_position.Y + 4, (int)rectWidth, (int)rectHeight),
                _sourceRect,
                tint,
                0,
                new Vector2(4, 4),
                SpriteEffects.None,
                1);
        }
    }
}
