using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects
{
    public class ClearFlash: VisualEffect
    {
        Color tint;
        float rectWidth;
        float rectHeight;

        public ClearFlash(Vector2 position, Color color, float timeDisplayed) : base(position)
        {
            this.position = position;
            tint = color;
            MaxTimeDisplayed = timeDisplayed;
            TimeDisplayed = timeDisplayed;
            image = GetContent.Load<Texture2D>("Image/Effect/clearEffect");
            rectWidth = image.Width;
            rectHeight = image.Height;
        }

        public override void Update(float deltaTime)
        {
            TimeDisplayed -= deltaTime;
            tint *= (TimeDisplayed / (MaxTimeDisplayed));
            rectWidth += 3f;
            rectHeight += 3f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw
                (image, 
                new Rectangle((int)position.X, (int)position.Y, (int)rectWidth, (int)rectHeight),
                null,
                tint,
                0,
                new Vector2(image.Width / 2, image.Height / 2),
                SpriteEffects.None,
                1);
        }
    }
}
