using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.VisualEffects.Text
{
    enum CounterState
    {
        Active,
        Increment,
        Fade,
        Inactive
    }
    public class CounterFlair: AnimatedEffect
    {
        public int count { get; private set; }
        private int _countBase;
        private int _deadZone;
        private (float timer, float timerMax) _flashDuration;
        private float _flashAmt;
        private (float timer, float timerMax) _fadeDuration;
        private float _fadeAmt;
        string _name;
        private Color _color;
        private (Color flash, Color regular) _colorSet;
        private CounterState _currentState;

        public CounterFlair(int counterBase, int deadZone, float flashDuration, float fadeDuration, string name, Color color, Vector2 position): base(position)
        {
            count = counterBase;
            _countBase = counterBase;
            _deadZone = deadZone;
            _flashDuration = (flashDuration, flashDuration);
            _fadeDuration = (fadeDuration, fadeDuration);
            _name = name;
            _colorSet = (Color.White, color);
            _currentState = CounterState.Inactive;
        }


        public void Ping(int num) 
        {
            _currentState = CounterState.Increment;
            _color = _colorSet.flash;
            count+= num;
        }

        public void SetPing(int num) 
        {
            _currentState = CounterState.Increment;
            _color = _colorSet.flash;
            count = num;
        }

        public void Reset() 
        {
            count = _countBase;
        }

        public void Kill() 
        {
            _currentState = CounterState.Fade;
        }

        public void Update(GameTime gameTime) 
        {
            switch (_currentState) 
            {
                case CounterState.Increment:
                    if (_flashDuration.timer <= 0) { _currentState = CounterState.Active; _flashDuration.timer = _flashDuration.timerMax; }
                    _flashDuration.timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _flashAmt = MathHelper.Clamp(_flashDuration.timer / _flashDuration.timerMax, 0, 1);
                    if(_color != _colorSet.regular)
                        _color = Color.Lerp(_colorSet.regular, _colorSet.flash, _flashAmt);
                    break;
                case CounterState.Fade:
                    if (_fadeDuration.timer <= 0) { _currentState = CounterState.Inactive; _fadeDuration.timer = _fadeDuration.timerMax; Reset(); }
                    _fadeDuration.timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _color *= (_fadeDuration.timer / _fadeDuration.timerMax);
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (count > _countBase + _deadZone)
                Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, position, $"{_name}{count}", _color, OriginSetting.BottomRight);
        }
    }
}
