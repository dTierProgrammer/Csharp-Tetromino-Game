using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.VisualEffects.Text
{
    public enum FlairState 
    {
        Active,
        Fading,
    }
    public class Flair: AnimatedEffect
    {
        private readonly RasterFont _font;
        private readonly string _str;
        private readonly (Color startColor, Color endColor) _colorTimeLine;
        private Color _color;
        private float _timeDisplayedAmt;
        public float fadeOutTime { get; private set; }
        private float _maxFadeOutTime;
        private float _fadeOutTimeAmt;
        private float _opacity;
        public FlairState currentState = FlairState.Active;
        private OriginSetting _originSetting;
        
        public Flair(Vector2 position, RasterFont font, string text, float timeDisplayed, float fadeOutTime, Color startColor, Color endColor, OriginSetting originSetting) : base(position) 
        {
            _font = font;
            _str = text;
            TimeDisplayed = timeDisplayed;
            MaxTimeDisplayed = timeDisplayed;
            _colorTimeLine = (startColor, endColor);
            _color = _colorTimeLine.startColor;
            this.fadeOutTime = fadeOutTime;
            _maxFadeOutTime = fadeOutTime;
            _originSetting = originSetting;
        }

        public Flair(RasterFont font, string text, float timeDisplayed, float fadeOutTime, Color startColor, Color endColor, OriginSetting originSetting)
        {
            _font = font;
            _str = text;
            TimeDisplayed = timeDisplayed;
            MaxTimeDisplayed = timeDisplayed;
            _colorTimeLine = (startColor, endColor);
            _color = _colorTimeLine.startColor;
            this.fadeOutTime = fadeOutTime;
            _maxFadeOutTime = fadeOutTime;
            _originSetting = originSetting;
        }

        public void Update(GameTime gameTime) 
        {
            switch (currentState) 
            {
                case FlairState.Active:
                    if (TimeDisplayed <= 0) { currentState = FlairState.Fading; _color = _colorTimeLine.endColor; }
                    TimeDisplayed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _timeDisplayedAmt = MathHelper.Clamp(TimeDisplayed / MaxTimeDisplayed, 0, 1);
                    _color = Color.Lerp(_colorTimeLine.endColor, _colorTimeLine.startColor, _timeDisplayedAmt);
                    break;
                case FlairState.Fading:
                    fadeOutTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _color *= (fadeOutTime / _maxFadeOutTime);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            _font.RenderString(spriteBatch, position, _str, _color, _originSetting);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            _font.RenderString(spriteBatch, pos, _str, _color, _originSetting);
        }
    }
}
