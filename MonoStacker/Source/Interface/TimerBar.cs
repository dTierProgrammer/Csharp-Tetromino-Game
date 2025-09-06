using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface
{
    public class TimerBar : ProgressBar
    {
        private (float timer, float maxTime) _time;
        private float _timerProgress;
        public event Action TimesUp;
        private bool _isActive = true;
        BarState _currentState;
        

        public TimerBar(Vector2 position, ProgressBarType type, float time) : base(position, type)
        {
            _currentState = BarState.Entrance;
            _position = position;
            _time = (time, time);
            segHeight = fillTexture.Height;
            segWidth = fillTexture.Width;
            _lerpTime = (.3f, .3f);
        }

        public void Update(GameTime gameTime) 
        {
            if (_time.timer <= 0) { TimesUp?.Invoke(); _isActive = false; }
            _time.timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!_isActive) return;
            switch (_type) 
            {
                case ProgressBarType.Vertical:
                    _timerProgress = MathHelper.Clamp(_time.timer / _time.maxTime, 0, 1);
                    segHeight = (int)MathHelper.Lerp(0, fillTexture.Height, _timerProgress);
                    break;
            }
        }
        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(fillTexture, new Vector2(_position.X, _position.Y + fillTexture.Height - segHeight), new Rectangle((int)_position.X, (int)_position.Y, fillTexture.Width, (int)segHeight), Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            spriteBatch.Draw(fillTexture, new Vector2(_position.X + drawOffset.X, _position.Y + fillTexture.Height - segHeight + drawOffset.Y), new Rectangle((int)_position.X, (int)_position.Y, fillTexture.Width, (int)segHeight), Color.White);
        }
    }
}
