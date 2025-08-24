using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects;
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
        private CounterFlair _streakCounter;
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
        private Texture2D bg;
        private float ms;
        private int snd;
        private int min;
        private bool _runTimer = true;
        private bool _runGame = true;
        private int piecesPlaced = 0;
        private KeyboardState _prevKBS;
        AnimatedEffectLayer _aeLayer = new();
        string _title = "Marathon Game";
        float startTimer;
        float intermediateTimer = 1;
        float successTimer = 2;
        

        public void Initialize() 
        {
            startTimer = 5;
            _currentState = GameState.PreGame;
            _playField = new PlayField(new Vector2(240, 135), new PlayFieldData(), new InputBinds());
            _atSys = new ActionTextSystem(new Vector2(_playField.offset.X - 13, _playField.offset.Y + 52));
            _comboCounter = new(-1, 1, .5f, .3f, "Combo *", Color.Orange, new(_playField.offset.X - 12, _playField.offset.Y + 41));
            _streakCounter = new(-1, 1, .5f, .3f, "Streak *", Color.Cyan, new(_playField.offset.X - 12, _playField.offset.Y + 49));
            _playField.ClearingLines += CheckForLines;
            _playField.ClearingLines += PingLineClear;
            _playField.ComboContinue += _comboCounter.Ping;
            _playField.ComboBreak += _comboCounter.Kill;
            _playField.StreakContinue += _streakCounter.Ping;
            _playField.StreakBreak += _streakCounter.Kill;
            _playField.TopOut += StopTimer;
            _playField.GameEnd += StopGame;
            _playField.PiecePlaced += IncrementPlacements;
            _playField.Bravo += BravoPing;
            _level = 1;
            _gravity = SetGravity(_level);
            _playField.gravity = _gravity;
            _levelProgressDisplay = new(new Vector2(_playField.offset.X - 7, _playField.offset.Y), _lineGoal, ProgressBarType.Vertical);
        }

        private void StopTimer() 
        {
            _runTimer = false;
        }

        private void StopGame() 
        {
            _runGame = false;
        }

        private void IncrementPlacements() 
        {
            piecesPlaced++;
        }

        private void RunTimer(GameTime gameTime) 
        {
            ms += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ms >= 1000) 
            {
                ms = 0;
                snd++;
            }

            if (snd >= 60) 
            {
                snd = 0;
                min++;
            } 
        }

        public void Load() 
        {
            bg = GetContent.Load<Texture2D>("Image/Background/bg_1080");
        }

        private float SetGravity(int level)
        {
            return level switch
            { // this sucks
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

        private float StreakMultiplier(int streak) 
        {
            return 1;
        }

        private void BravoPing() 
        {
            _aeLayer.AddEffect(new EventTitle(ImgBank.BravoTitle, new Vector2(240, 100), new Vector2(ImgBank.BravoTitle.Width + 100, 0), .2f, 1f, new Vector2(ImgBank.BravoTitle.Width + 50, ImgBank.BravoTitle.Height + 10), 1f));
        }

        private void PingLineClear() 
        {
            string lineClearTitle = _playField.grid.rowsToClear.Count switch
            {
                1 => "single!",
                2 => "double!!",
                3 => "triple!!!",
                4 => "quadruple!!!!",
                _ => "super move!!!!!"
            };

            string spinTitle = _playField.currentSpinType switch
            {
                SpinType.None => "",
                SpinType.MiniSpin => $"Mini {(char)_playField.activePiece.type}-spin",
                SpinType.FullSpin => $"{(char)_playField.activePiece.type}-spin"
            };

            (Color color1, Color color2) lineClearColor = _playField.grid.rowsToClear.Count switch
            {
                1 => (Color.Lime, Color.Yellow),
                2 => (Color.Yellow, Color.Orange),
                3 => (Color.Orange, Color.OrangeRed),
                4 => (Color.OrangeRed, Color.Red),
                _ => (Color.LightBlue, Color.Blue)
            };

            _atSys.Ping($"{spinTitle} {lineClearTitle}", _playField.currentSpinType is SpinType.None ? lineClearColor.color1 : _playField.activePiece.color, _playField.currentSpinType is SpinType.None ? lineClearColor.color2 : Color.White, 3f, .5f);
        }

        private void CheckForLines()
        {
            _goalProgress += _playField.grid.rowsToClear.Count;
            _linesCleared += _playField.grid.rowsToClear.Count;
            var num = _streakCounter.count > 0 ? _streakCounter.count : 1;
            if (_playField.currentSpinType == SpinType.None)
            {
                _score += _playField.grid.rowsToClear.Count switch
                {
                    1 => (100 * _level) + (50 * _comboCounter.count * _level),
                    2 => (300 * _level) + (50 * _comboCounter.count * _level),
                    3 => (500 * _level) + (50 * _comboCounter.count * _level),
                    4 => (800 * _level) + (50 * _comboCounter.count * _level) * num
                };
            }
            else if (_playField.currentSpinType == SpinType.MiniSpin)
            {
                _score += _playField.grid.rowsToClear.Count switch
                {
                    1 => (200 * _level) + (50 * _comboCounter.count * _level) * num,
                    2 => (400 * _level) + (50 * _comboCounter.count * _level) * num,
                    3 => (600 * _level) + (50 * _comboCounter.count * _level) * num,
                    4 => (800 * _level) + (50 * _comboCounter.count * _level) * num
                };
            }
            else 
            {
                _score += _playField.grid.rowsToClear.Count switch
                {
                    1 => (800 * _level) + (50 * _comboCounter.count * _level) * num,
                    2 => (1200 * _level) + (50 * _comboCounter.count * _level) * num,
                    3 => (1600 * _level) + (50 * _comboCounter.count * _level) * num,
                    4 => (2000 * _level) + (50 * _comboCounter.count * _level) * num
                };
            }

            ValidateProgress();
            _levelProgressDisplay.Update(_goalProgress);
        }

        private void ValidateProgress() 
        {
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
                    _atSys.Ping("level up!", Color.Cyan, Color.RoyalBlue,  3, .3f);
                }
            }
        }

        private void StartingCountDown(GameTime gameTime) 
        {
            if (startTimer <= 1) { _currentState = GameState.Play; _playField.Start(); AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.Go, new Vector2(240, 100), new Vector2(ImgBank.Go.Width + 100, ImgBank.Go.Height + 90), .2f, .5f, Vector2.Zero, 1f, Color.OrangeRed, Color.OrangeRed, Color.Red, false)); }
            startTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (intermediateTimer <= 0)
            {
                switch ((int)startTimer)
                {
                    case 1:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown1, new Vector2(240, 100), new Vector2(ImgBank.CountDown1.Width + 100, ImgBank.CountDown1.Height + 90), .2f, .5f, Vector2.Zero, 1f, Color.OrangeRed, Color.White, Color.White, false));
                        break;
                    case 2:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown2, new Vector2(240, 100), new Vector2(ImgBank.CountDown2.Width + 100, ImgBank.CountDown2.Height + 90), .2f, .5f, Vector2.Zero, 1f, Color.Orange, Color.White, Color.White, false));
                        break;
                    case 3:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown3, new Vector2(240, 100), new Vector2(ImgBank.CountDown3.Width + 100, ImgBank.CountDown3.Height + 90), .2f, .5f, Vector2.Zero, 1f, Color.Yellow, Color.White, Color.White, false));
                        break;
                }
                intermediateTimer = 1;
            }
            intermediateTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void UpdateGame(GameTime gameTime) 
        {
            _playField.Update(gameTime);
        }

        public void Update(GameTime gameTime) 
        {
            switch (_currentState) 
            {
                case GameState.PreGame:
                    StartingCountDown(gameTime);
                    break;
                case GameState.Play:
                    if(_runGame)
                        UpdateGame(gameTime);
                    if (_runTimer) 
                        RunTimer(gameTime);
                    if (_linesCleared >= _maxLinesCleared) 
                    {
                        if (successTimer == 2) 
                        { 
                            AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.ClearTitle, new Vector2(240, 100), new Vector2(ImgBank.ClearTitle.Width + 100, 0), .2f, 1f, new Vector2(ImgBank.ClearTitle.Width + 50, ImgBank.ClearTitle.Height + 10), 1f));
                            _runTimer = false;
                            _playField.PauseForEvent();
                        }
                        successTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (successTimer <= 0) 
                        {
                            _currentState = GameState.Succeed;
                            _playField.End();
                        }
                    }
                    break;
                case GameState.Succeed:
                    if(_runGame)
                        UpdateGame(gameTime);
                    break;
                case GameState.GameOver:
                    break;
                case GameState.Pause:
                    break;
            }
            _atSys.Update(gameTime);
            _comboCounter.Update(gameTime);
            _streakCounter.Update(gameTime);

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && !_prevKBS.IsKeyDown(Keys.LeftAlt)) 
                _linesCleared += 30;
            _prevKBS = Keyboard.GetState();
#endif
            _aeLayer.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);

            spriteBatch.End();
            _playField.Draw(spriteBatch);
            spriteBatch.Begin();
            _levelProgressDisplay.Draw(spriteBatch);
# if DEBUG
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, Vector2.Zero, 
                $"lines -- {_linesCleared}\n" +
                $"progress -- {_goalProgress}\n" +
                $"level -- {_level}\n" +
                $"progress Amt -- {_goalProgressAmt}",
                Color.Orange, OriginSetting.TopLeft);
#endif
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(285, 186), $"Score: {_score:000000000}", Color.Orange, OriginSetting.TopLeft);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(285, 194), $"Time: {min:00}:{snd:00}", Color.Orange, OriginSetting.TopLeft);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(188, 186), $"Lines: {_linesCleared}/{_maxLinesCleared}", Color.Orange, OriginSetting.TopRight);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(188, 194), $"Level {_level}", Color.Orange, OriginSetting.TopRight);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(188, 202), $"Pieces: {piecesPlaced}", Color.Orange, OriginSetting.TopRight);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(240, 230), $"{_title}", Color.Lime, OriginSetting.Top);
            spriteBatch.End();
            _atSys.Draw(spriteBatch);
            spriteBatch.Begin();
            _comboCounter.Draw(spriteBatch);
            _streakCounter.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin();
            _aeLayer.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
