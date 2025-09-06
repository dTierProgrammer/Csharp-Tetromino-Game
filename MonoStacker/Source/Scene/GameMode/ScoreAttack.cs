using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Data;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.VisualEffects;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoStacker.Source.Scene.GameMode
{
    public class ScoreAttack: MarathonMode
    {
        private int _totalSeconds;
 
        private TimerBar _timerBar;
        protected override void InitRuleset()
        {
            pfData = PlayFieldPresets.GuidelineFast1;
            _lineGoal = int.MaxValue;
            capLinesCleared = false;
        }
        protected override void InitClocks()
        {
            base.InitClocks();
            clockBehavior = ClockBehavior.Decrement;
            time = 180;
        }

        protected override void InitProgressBar()
        {
            _levelProgressDisplay = new(new Microsoft.Xna.Framework.Vector2(_playField.Offset.X - 7, _playField.Offset.Y),_totalSeconds, _totalSeconds, ProgressBarType.Vertical);
            _timerBar = new(new Microsoft.Xna.Framework.Vector2(_playField.Offset.X - 7, _playField.Offset.Y), ProgressBarType.Vertical, time);
        }

        public override void Initialize()
        {
            base.Initialize();
            _title = $"Score Attack -- {TimeSpan.FromSeconds(time).ToString(@"mm\:ss")}";
        }

        protected override void DecrementTimer(GameTime gameTime) 
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        protected override void IncrementScore()
        {
            _goalProgress += _playField.grid.rowsToClear.Count;
            _linesCleared += _playField.grid.rowsToClear.Count;
            if (_playField.currentSpinType == SpinType.None)
            {
                _score += (_playField.grid.rowsToClear.Count switch
                {
                    1 => (100 * _level),
                    2 => (300 * _level),
                    3 => (500 * _level),
                    4 => (800 * _level),
                    _ => (1100 * _level * _playField.grid.rowsToClear.Count)
                } + ComboMultiplier(_comboCounter.count) * StreakMultiplier(_streakCounter.count));
            }
            else if (_playField.currentSpinType == SpinType.MiniSpin)
            {
                _score += (_playField.grid.rowsToClear.Count switch
                {
                    1 => (200 * _level),
                    2 => (400 * _level),
                    3 => (600 * _level),
                    4 => (800 * _level),
                    _ => (1200 * _level * _playField.grid.rowsToClear.Count)
                } + ComboMultiplier(_comboCounter.count)) * StreakMultiplier(_streakCounter.count);
            }
            else
            {
                _score += (_playField.grid.rowsToClear.Count switch
                {
                    1 => (800 * _level),
                    2 => (1200 * _level),
                    3 => (1600 * _level),
                    4 => (2000 * _level),
                    _ => (2500 * _level * _playField.grid.rowsToClear.Count)
                } + ComboMultiplier(_comboCounter.count)) * StreakMultiplier(_streakCounter.count);
            }
            if (_score > 999999999) _score = 9999999999;

            ValidateProgress();
        }

        protected override void UpdatePlaying(GameTime gameTime) 
        {
            if(_runGame)
                UpdatePlayfield(gameTime);
            if (_runTimer)
            { RunTimer(gameTime); _timerBar.Update(gameTime); }
            if (time <= 0)
            { UpdateClearState(gameTime); }
            
        }

        protected override void UpdateClearState(GameTime gameTime)
        {
            if (successTimer == 2)
            {
                AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.TimeTitle, new Vector2(240, 100), new Vector2(ImgBank.ClearTitle.Width * 2, 0), .2f, 1f, Vector2.Zero, 1f, Color.White, Color.White, Color.White, false));
                _runTimer = false;
                _playField.PauseForEvent();
                _streakCounter.Kill();
                _comboCounter.Kill();
            }
            successTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (successTimer <= 0)
            {
                _currentState = GameState.Succeed;
                _playField.End();
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();

            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);
            //Debug.WriteLine("everything else");
            spriteBatch.End();
            _particleLayer.Draw(spriteBatch);
            _playField.Draw(spriteBatch);
            //Debug.WriteLine("everything else");
            spriteBatch.Begin();

            //_levelProgressDisplay.Draw(spriteBatch);
            _timerBar.Draw(spriteBatch, new Vector2((int)_playField._shakeOffsetX, (int)_playField._shakeOffsetY));
            
# if DEBUG
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, Vector2.Zero,
                $"lines -- {_linesCleared}\n" +
                $"progress -- {_goalProgress}\n" +
                $"level -- {_level}\n" +
                $"progress Amt -- {_goalProgressAmt}",
                Color.Orange, OriginSetting.TopLeft);
#endif

            spriteBatch.End();
            _atSys.Draw(spriteBatch);
            spriteBatch.Begin();
            _aeLayer.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
