using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStacker.Source.VisualEffects
{
    public abstract class AnimatedEffect
    {
        public float TimeDisplayed { get; protected set; }
        public float MaxTimeDisplayed { get; protected set; }
        public Texture2D image { get; protected set; }
        public virtual Vector2 position { get; protected set; }

        public AnimatedEffect(Vector2 position) { this.position = position; }

        protected AnimatedEffect() { }

        public virtual void Update(float deltaTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
