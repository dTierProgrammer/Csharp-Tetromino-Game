using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects.Text;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Scene.GameMode
{
    class MarathonMode: IScene
    {
        private GameState _currentState;
        private PlayField _playField;
        private float _score = 0;
        private CounterFlair _comboCounter;
        private CounterFlair _scoreCounter;
        private ActionTextSystem _atSys;
        private float _gravity;
        private float _prevLinesCleared;
        private int _linesCleared = 0;
        private int? _maxLinesCleared = 150;
        private int _level = 0;
        private int _maxLevel = 20;
        private int _lineGoal = 10;
        private int _goalProgress = 0;
        private float _goalProgressAmt;
        private ProgressBar _levelProgressDisplay;

        public void Initialize() 
        {
            _currentState = GameState.Play;
            _playField = new PlayField(new Vector2(230, 135), new PlayFieldData(), new InputBinds());
            _level = 1;
            _gravity = SetGravity(_level);
            _playField.gravity = _gravity;
            _levelProgressDisplay = new(new Vector2(_playField.offset.X - 7, _playField.offset.Y), _lineGoal, ProgressBarType.Vertical);
        }

        private float SetGravity(int level)
        {
            return level switch
            {
                1 => 0.01667f,
                2 => 0.021017f,
                3 => 0.026977f,
                4 => 0.035256f,
                5 => 0.04693f,
                6 => 0.06361f,
                7 => 0.0879f,
                8 => 0.1236f,
                9 => 0.1775f,
                10 => 0.2598f,
                11 => 0.388f,
                12 => 0.59f,
                13 => 0.92f,
                14 => 1.46f,
                15 => 2.36f,
                16 => 3.91f,
                17 => 6.61f,
                18 => 11.43f,
                19 => 20.23f,
                _ => 36.6f
            };
        }

        private void ValidateProgress() 
        {
            if(_playField.grid.rowsToClear.Count > 0)
                _goalProgress += _playField.grid.rowsToClear.Count;

            if (_goalProgress >= _lineGoal) 
            {
                var gp = _goalProgress;
                _goalProgress -= _lineGoal;
                if (_level > 20) return;
                for (int i = 1; i <= gp; i++)
                {
                    if (i % _lineGoal != 0) continue;
                    _level++;
                    _playField.gravity = SetGravity(_level);
                }
            }
        }

        public void Load() { }
        public void Update(GameTime gameTime) 
        {
            switch (_currentState) 
            {
                case GameState.PreGame:
                    break;
                case GameState.Play:
                    _playField.Update(gameTime);
                    _linesCleared = _playField.linesCleared;
                    if(_linesCleared != _prevLinesCleared)
                        ValidateProgress();
                    _levelProgressDisplay.Update(_goalProgress);
                    _prevLinesCleared = _linesCleared;
                    break;
                case GameState.Succeed:
                    break;
                case GameState.GameOver:
                    break;
                case GameState.Pause:
                    break;
            }
            Debug.WriteLine(_linesCleared);
        }
        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Background/bg_1080"), new Vector2(0, 0), Color.White);

            spriteBatch.End();
            _playField.Draw(spriteBatch);
            spriteBatch.Begin();
            _levelProgressDisplay.Draw(spriteBatch);
            //spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Font/flair_bg"), new Rectangle((int)_playField.offset.X - 7, (int)_playField.offset.Y, 3, (int)MathHelper.Lerp(0, 160, _goalProgressAmt)), Color.Orange);
# if DEBUG
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, Vector2.Zero, 
                $"lines -- {_linesCleared}\n" +
                $"progress -- {_goalProgress}\n" +
                $"level -- {_level}\n" +
                $"progress Amt -- {_goalProgressAmt}",
                Color.Orange, OriginSetting.TopLeft);
#endif
            spriteBatch.End();
        }
    }
}
