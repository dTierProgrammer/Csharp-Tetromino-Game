using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface
{
    public enum ProgressBarType 
    {
        Vertical,
        Horizontal
    }

    enum BarState 
    {
        Entrance,
        Active,
        Inactive
    }
    public class ProgressBar
    {
        protected Vector2 _position;
        protected float _max;
        protected float _current = 0;
        protected (Color color1, Color color2) _colorSet;
        protected Color _color;
        protected Texture2D fillTexture = GetContent.Load<Texture2D>("Image/Board/levelMeterFill");
        protected Texture2D animatedTexture = GetContent.Load<Texture2D>("Image/Board/levelMeterFill");
        protected Rectangle segment;
        protected Rectangle anSegment;
        protected float segWidth;
        protected float segHeight;
        protected float an_segWidth;
        protected float an_segHeight;
        protected (float timer, float maxTime) _lerpTime = (1.8f, 1.8f);
        protected float _lerpTimeProgress;
        protected ProgressBarType _type;

        public ProgressBar(Vector2 position, float max, ProgressBarType type) 
        {
            _position = position;
            _max = max;
            _type = type;
            segment = type switch 
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };

            anSegment = type switch 
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };

        }

        public ProgressBar(Vector2 position, int start, float max, ProgressBarType type)
        {
            _position = position;
            _max = max;
            _type = type;
            _current = start;
            segment = type switch
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };

            anSegment = type switch
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };
        }

        public ProgressBar(Vector2 position, ProgressBarType type) 
        {
            _type = type;
            segment = type switch
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };

            anSegment = type switch
            {
                ProgressBarType.Vertical => new Rectangle((int)position.X, (int)position.Y, fillTexture.Width, 0),
                ProgressBarType.Horizontal => new Rectangle((int)position.X, (int)position.Y, 0, fillTexture.Height)
            };
        }

        public void Update(float value) 
        {
            _current = value;
            switch (_type) 
            {
                case ProgressBarType.Vertical:
                    segment.Height = (int)((_current / _max) * fillTexture.Height);
                    break;
                case ProgressBarType.Horizontal:
                    segment.Width = (int)((_current / _max) * fillTexture.Width);
                    break;
            }
            _lerpTime.timer = _lerpTime.maxTime;
            if (anSegment.Height > segment.Height) anSegment.Height = 0;
        }

        public void UpdateAnimation(GameTime gameTime) 
        {
            if (anSegment.Height < segment.Height) 
            {
                _lerpTime.timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                float lerpProgress = MathHelper.Clamp(_lerpTime.timer / _lerpTime.maxTime, 0, 1);
                anSegment.Height = (int)MathHelper.Lerp(segment.Height, anSegment.Height, lerpProgress * lerpProgress);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(fillTexture, new Vector2(_position.X, _position.Y + fillTexture.Height - segment.Height), segment, Color.DarkGray);
            spriteBatch.Draw(animatedTexture, new Vector2(_position.X, _position.Y + fillTexture.Height - anSegment.Height), anSegment, Color.White);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            spriteBatch.Draw(fillTexture, new Vector2(_position.X + drawOffset.X, _position.Y + fillTexture.Height - segment.Height + drawOffset.Y), segment, Color.DarkGray);
            spriteBatch.Draw(animatedTexture, new Vector2(_position.X + drawOffset.X, _position.Y + fillTexture.Height - anSegment.Height + drawOffset.Y), anSegment, Color.White);
        }
    }
}
