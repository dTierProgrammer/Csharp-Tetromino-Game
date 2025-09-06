using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
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
    public enum ClockBehavior
    {
        Increment,
        Decrement,
        Disable
    }

    public enum LineCap 
    {
        Enable,
        Disable
    }

    public class MarathonMode: IScene
    {
        protected GameState _currentState;
        protected PlayField _playField;
        protected PlayFieldData pfData;
        protected float _score = 0;
        protected CounterFlair _comboCounter;
        protected CounterFlair _streakCounter;
        protected CounterFlair _scoreCounter;
        protected ActionTextSystem _atSys;
        protected float _gravity;
        protected bool capLinesCleared;
        protected int _linesCleared = 0;
        protected int? _maxLinesCleared = 150;
        protected int _level = 0;
        protected int _maxLevel = 20;
        protected int _lineGoal = 10;
        protected int _goalProgress = 0;
        protected float _goalProgressAmt;
        protected ProgressBar _levelProgressDisplay;
        protected Texture2D bg;
        protected float ms;
        protected int snd;
        protected int min;
        protected float time;
        protected bool _runTimer = true;
        protected bool _runGame = true;
        protected int piecesPlaced = 0;
        protected KeyboardState _prevKBS;
        protected AnimatedEffectLayer _aeLayer = new();
        protected string _title = "Marathon Game";
        protected List<string> leftStatsDisplay = new();
        protected List<string> rightStatsDisplay = new();
        protected float startTimer;
        protected float intermediateTimer = 1;
        protected float successTimer = 2;
        protected ClockBehavior clockBehavior;

        protected StaticEmissionSource _streakFireSource;
        protected EmitterObj _streakFireEmitter;
        protected EmitterData _streakFire;
        protected ParticleLayer _particleLayer = new();

        protected StaticEmissionSource _comboFireSource;
        protected EmitterObj _comboFireEmitter;
        protected EmitterData _comboFire;

        protected StaticEmissionSource _levelUpEffectSource;
        protected EmitterData _levelUpEffect;
        protected EmitterObj _levelUpEffectEmitter;

        protected StaticEmissionSource _testEffectSource;
        protected EmitterData _testEffect;
        protected EmitterObj _testEffectEmitter;

        protected virtual void InitEvents() 
        {
            _playField.ClearingLines += IncrementScore;
            _playField.ClearingLines += PingLineClear;
            _playField.GenericSpinPing += PingLineClear;
            _playField.ComboContinue += _comboCounter.Ping;
            _playField.ComboBreak += _comboCounter.Kill;
            _playField.StreakContinue += _streakCounter.Ping;
            _playField.StreakBreak += _streakCounter.Kill;
            _playField.TopOut += StopTimer;
            _playField.GameEnd += StopGame;
            _playField.PiecePlaced += IncrementPlacements;
            _playField.Bravo += BravoPing;
        }

        protected virtual void InitEffects() 
        {
            _streakFireSource = new(new(_playField.offset.X - 48, _playField.offset.Y + 46));
            _streakFire = new EmitterData()
            {
                particleData = new ParticleData
                {
                    //texture = GetContent.Load<Texture2D>("Image/Effect/Particle/default"),
                    angle = 330,
                    opacityTimeLine = new(1f, 0f),
                    scaleTimeLine = new(8, 1),
                    colorTimeLine = (Color.Cyan, Color.Blue),
                    rotationSpeed = .01f
                },
                angleVarianceMax = 3,
                particleActiveTime = (1, 3),
                emissionInterval = .1f,
                speed = (1, 9),
                density = 3,
                rotationSpeed = (-.03f, .03f)
            };
            _streakFireEmitter = new EmitterObj(_streakFireSource, _streakFire, EmissionType.Continuous, false);
            _particleLayer.AddEmitter(_streakFireEmitter);

            _comboFireSource = new(new(_playField.offset.X - 45, _playField.offset.Y + 38));
            _comboFire = new EmitterData()
            {
                particleData = new ParticleData
                {
                    //texture = GetContent.Load<Texture2D>("Image/Effect/Particle/default"),
                    angle = 330,
                    opacityTimeLine = new(1f, 0f),
                    scaleTimeLine = new(8, 1),
                    colorTimeLine = (Color.Orange, Color.Red),
                    rotationSpeed = .01f
                },
                angleVarianceMax = 3,
                particleActiveTime = (1, 3),
                emissionInterval = .1f,
                speed = (1, 9),
                density = 3,
                rotationSpeed = (-.03f, .03f)
            };
            _comboFireEmitter = new EmitterObj(_comboFireSource, _comboFire, EmissionType.Continuous, false);
            _particleLayer.AddEmitter(_comboFireEmitter);

            _levelUpEffectSource = new(new(_playField.offset.X - 7, _playField.offset.Y + 160));
            _levelUpEffect = new EmitterData()
            {
                particleData = new ParticleData
                {
                    texture = GetContent.Load<Texture2D>("Image/Effect/Particle/starLarge"),
                    opacityTimeLine = new(1f, 0f),
                    scaleTimeLine = new(4, 4),
                    colorTimeLine = (Color.White, Color.White),
                },
                angleVarianceMax = 90,
                particleActiveTime = (.5f, 1),
                speed = (20, 50),
                density = 50,
                offsetY = (0, -160),
                offsetX = (0, 3),
                rotationSpeed = (-.03f, .03f)
            };
            _levelUpEffectEmitter = new EmitterObj(_levelUpEffectSource, _levelUpEffect, EmissionType.Burst);

            _testEffectSource = new(new Vector2(100, 100));
            _testEffect = new EmitterData() 
            {
                particleData = new ParticleData() 
                {
                    texture = GetContent.Load<Texture2D>("Image/Effect/Particle/squareEmpty"),
                    opacityTimeLine = new(1, 0),
                    scaleTimeLine = new(8, 64),
                    colorTimeLine = (Color.Blue, Color.Cyan),
                    rotationSpeed = .1f
                },
                speed = (0, 0),
                density = 1,
                emissionInterval = .15f,
                particleActiveTime = (.5f, .5f),
                activeTimeLeft = .45f
            };
            _testEffectEmitter = new(_testEffectSource, _testEffect, EmissionType.Timed);
        }

        protected virtual void InitClocks() 
        {
            startTimer = 5;
            clockBehavior = ClockBehavior.Increment;
            ms = 0;
            snd = 0;
            min = 0;
            time = 0;
        }

        protected virtual void InitProgressBar() 
        {
            _levelProgressDisplay = new(new Vector2(_playField.offset.X - 7, _playField.offset.Y), _lineGoal, ProgressBarType.Vertical);
        }

        protected virtual void InitRuleset() 
        {
            pfData = PlayFieldPresets.GuidelineSlow1;
            capLinesCleared = true;
        }

        public virtual void Initialize() 
        {
            InitRuleset();
            _currentState = GameState.PreGame;
            _playField = new PlayField(new Vector2(240, 135), pfData, new InputBinds());
            _atSys = new ActionTextSystem(new Vector2(_playField.offset.X - 13, _playField.offset.Y + 52));
            _comboCounter = new(-1, 1, .5f, .3f, "Combo *", Color.Orange, new(_playField.offset.X - 13, _playField.offset.Y + 41));
            _streakCounter = new(-1, 1, .5f, .3f, "Streak *", Color.Cyan, new(_playField.offset.X - 13, _playField.offset.Y + 51));
            _level = 1;
            _gravity = SetGravity(_level);
            _playField.gravity = _gravity;
           
            InitClocks();
            InitEvents();
            InitEffects();
            InitProgressBar();
            Debug.WriteLine(clockBehavior);
        }

        protected void StopTimer() 
        {
            _runTimer = false;
        }

        protected void StopGame() 
        {
            _runGame = false;
        }

        protected void IncrementPlacements() 
        {
            piecesPlaced++;
        }

        protected virtual void IncrementTimer(GameTime gameTime) 
        {
            ms += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
        protected virtual void DecrementTimer(GameTime gameTime) 
        {
            ms -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ms < 0)
            {
                ms = 999;
                snd--;
            }

            if (snd < 0)
            {
                snd = 59;
                min--;
            }
        }

        protected void RunTimer(GameTime gameTime) 
        {
            switch (clockBehavior) 
            {
                case ClockBehavior.Increment:
                    IncrementTimer(gameTime);
                    break;
                case ClockBehavior.Decrement:
                    DecrementTimer(gameTime);
                    break;
            }
            
        }

        public void Load() 
        {
            bg = GetContent.Load<Texture2D>("Image/Background/bg_1080");
        }

        protected float SetGravity(int level)
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

        protected float StreakMultiplier(int streak) 
        {
            return streak switch 
            {
                -1 => 1,
                0 => 1,
                _ => streak
            };
        }

        protected float ComboMultiplier(int combo) 
        {
            return combo switch 
            {
                -1 => 0,
                0 => 0,
                _ => 50 * combo * _level
            };
        }

        protected void BravoPing() 
        {
            _aeLayer.AddEffect(new EventTitle(ImgBank.BravoTitle, new Vector2(240, 100), new Vector2(ImgBank.BravoTitle.Width + 100, 0), .2f, 1f, new Vector2(ImgBank.BravoTitle.Width + 50, ImgBank.BravoTitle.Height + 10), 1f));
        }

        protected void PingLineClear() 
        {
            string lineClearTitle = _playField.grid.rowsToClear.Count switch
            {
                0 => "!",
                1 => " single!",
                2 => " double!!",
                3 => " triple!!!",
                4 => " quadruple!!!!",
                _ => " super move!!!!!"
            };

            string spinTitle = _playField.currentSpinType switch
            {
                SpinType.MiniSpin => $"Mini {ConvertTypeValue.GetTypeChar(_playField.activePiece.type)}-spin",
                SpinType.FullSpin => $"{ConvertTypeValue.GetTypeChar(_playField.activePiece.type)}-spin",
                _ => "",
            };

            (Color color1, Color color2) lineClearColor = _playField.grid.rowsToClear.Count switch
            {
                1 => (Color.Lime, Color.Yellow),
                2 => (Color.Yellow, Color.Orange),
                3 => (Color.Orange, Color.OrangeRed),
                4 => (Color.OrangeRed, Color.Red),
                _ => (Color.LightBlue, Color.Blue)
            };

            if (_playField.parsedSpins is not SpinDenotation.TSpinSpecific || (_playField.parsedSpins is SpinDenotation.TSpinSpecific && _playField.activePiece.type is TetrominoType.T || _playField.currentSpinType is SpinType.None && _playField.activePiece.type is not TetrominoType.T))
                _atSys.Ping($"{spinTitle}{lineClearTitle}", _playField.currentSpinType is SpinType.None ? lineClearColor.color1 : _playField.activePiece.color, _playField.currentSpinType is SpinType.None ? lineClearColor.color2 : Color.White, 3f, .5f);
            else if (_playField.parsedSpins is SpinDenotation.TSpinSpecific && _playField.activePiece.type is not TetrominoType.T && _playField.currentSpinType is not SpinType.None)
            {
                lineClearTitle = _playField.grid.rowsToClear.Count switch
                {
                    0 => "!",
                    1 => "!",
                    2 => "!!",
                    3 => "!!!",
                    4 => "!!!!",
                    _ => "!!!!!--"
                };
                _atSys.Ping($"super move{lineClearTitle}", Color.LightBlue, Color.Blue, 3f, .5f); 
            }
        }

        protected virtual void IncrementScore()
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
                    _ => (1100 * _level * _playField.grid.rowsToClear.Count)
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
            _levelProgressDisplay.Update(_goalProgress);
        }

        protected void ValidateProgress() 
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
                    _aeLayer.AddEffect(new LockFlash
                        (
                            GetContent.Load<Texture2D>("Image/Board/levelMeterFillMask"), 
                            new Vector2(_playField.offset.X - 7, _playField.offset.Y),
                            Color.White,
                            5f
                        ));
                    _particleLayer.AddEmitter(_levelUpEffectEmitter);
                }
            }
        }

        protected void StartingCountDown(GameTime gameTime) 
        {
            if (startTimer <= 1) { _currentState = GameState.Play; _playField.Start(); AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.Go, new Vector2(240, 100), new Vector2(ImgBank.Go.Width * 3, ImgBank.Go.Height * 3), .2f, .5f, Vector2.Zero, 1f, Color.OrangeRed, Color.OrangeRed, Color.Red, false)); }
            startTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (intermediateTimer <= 0)
            {
                switch ((int)startTimer)
                {
                    case 1:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown1, new Vector2(240, 100), new Vector2(ImgBank.CountDown1.Width * 2f, ImgBank.CountDown1.Height * 2f), .2f, .5f, Vector2.Zero, 1f, Color.OrangeRed, Color.White, Color.White, false));
                        break;
                    case 2:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown2, new Vector2(240, 100), new Vector2(ImgBank.CountDown2.Width * 2f, ImgBank.CountDown2.Height * 2f), .2f, .5f, Vector2.Zero, 1f, Color.Orange, Color.White, Color.White, false));
                        break;
                    case 3:
                        AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.CountDown3, new Vector2(240, 100), new Vector2(ImgBank.CountDown3.Width * 2f, ImgBank.CountDown3.Height * 2f), .2f, .5f, Vector2.Zero, 1f, Color.Yellow, Color.White, Color.White, false));
                        break;
                }
                intermediateTimer = 1;
            }
            intermediateTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected void UpdatePlayfield(GameTime gameTime) 
        {
            _playField.Update(gameTime);

            if (_streakCounter.count >= 8)
                _streakFireEmitter._emitParticles = true;
            else
                _streakFireEmitter._emitParticles = false;
            if (_comboCounter.count >= 8)
                _comboFireEmitter._emitParticles = true;
            else
                _comboFireEmitter._emitParticles = false;
        }

        protected virtual void UpdateClearState(GameTime gameTime) 
        {
            if (successTimer == 2)
            {
                AnimatedEffectManager.AddEffect(new EventTitle(ImgBank.ClearTitle, new Vector2(240, 100), new Vector2(ImgBank.ClearTitle.Width * 2, 0), .2f, 1f, new Vector2(ImgBank.ClearTitle.Width * 1.5f, ImgBank.ClearTitle.Height * 1.5f), 1f));
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

        protected virtual void UpdatePlaying(GameTime gameTime) 
        {
            if (_runGame)
                UpdatePlayfield(gameTime);
            if (_runTimer)
                RunTimer(gameTime);
            if (_linesCleared >= _maxLinesCleared)
                UpdateClearState(gameTime);
        }

        public void Update(GameTime gameTime) 
        {
            switch (_currentState) 
            {
                case GameState.PreGame:
                    StartingCountDown(gameTime);
                    break;
                case GameState.Play:
                    UpdatePlaying(gameTime);
                    break;
                case GameState.Succeed:
                    if(_runGame)
                        UpdatePlayfield(gameTime);
                    break;
                case GameState.GameOver:
                    break;
                case GameState.Pause:
                    break;
            }
            _atSys.Update(gameTime);
            _comboCounter.Update(gameTime);
            _streakCounter.Update(gameTime);
            _levelProgressDisplay.UpdateAnimation(gameTime);

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && !_prevKBS.IsKeyDown(Keys.LeftAlt)) 
                _linesCleared += 30;
            if (Keyboard.GetState().IsKeyDown(Keys.RightShift) && !_prevKBS.IsKeyDown(Keys.RightShift))
                _particleLayer.AddEmitter(_levelUpEffectEmitter);
            if (Keyboard.GetState().IsKeyDown(Keys.RightControl) && !_prevKBS.IsKeyDown(Keys.RightControl))
                _particleLayer.AddEmitter(new(_testEffectSource, _testEffect, EmissionType.Timed));

            _prevKBS = Keyboard.GetState();
#endif
            _aeLayer.Update(gameTime);
            _particleLayer.Update(gameTime);
            //Debug.WriteLine(clockBehavior);
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            
            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);
            //Debug.WriteLine("everything else");
            spriteBatch.End();
            _particleLayer.Draw(spriteBatch);
            _playField.Draw(spriteBatch);
            //Debug.WriteLine("everything else");
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
            
            spriteBatch.End();
            _atSys.Draw(spriteBatch);
            spriteBatch.Begin();
            _aeLayer.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void DrawText(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            _comboCounter.Draw(spriteBatch);
            _streakCounter.Draw(spriteBatch);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(240, 230), $"{_title}", Color.Lime, OriginSetting.Top);

            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(286, 186), $"Score: {_score:000000000}", Color.Orange, OriginSetting.TopLeft);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(286, 194), $"Time: {TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff")}", Color.Orange, OriginSetting.TopLeft);
            if(capLinesCleared)
                Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(187, 186), $"Lines: {_linesCleared}/{_maxLinesCleared}", Color.Orange, OriginSetting.TopRight);
            else
                Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(187, 186), $"Lines: {_linesCleared}", Color.Orange, OriginSetting.TopRight);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(187, 194), $"Level {_level}", Color.Orange, OriginSetting.TopRight);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(187, 202), $"Pieces: {piecesPlaced}", Color.Orange, OriginSetting.TopRight);
            spriteBatch.End();
        }
    }
}
