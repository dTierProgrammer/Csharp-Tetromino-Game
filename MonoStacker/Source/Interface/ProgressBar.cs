using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface
{
    public enum ProgressBarType 
    {
        Vertical,
        Horizontal
    }
    public class ProgressBar
    {
        private Vector2 _position;
        private float _max;
        private float _current = 0;
        private (Color color1, Color color2) _colorSet;
        private Color _color;
        private Texture2D fillTexture = GetContent.Load<Texture2D>("Image/Board/levelMeterFill");
        private Rectangle segment;
        private float segWidth;
        private float segHeight;
        ProgressBarType _type;

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
        }
        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(fillTexture, new Vector2(_position.X, _position.Y + fillTexture.Height - segment.Height), segment, Color.White);
        }
    }
}
