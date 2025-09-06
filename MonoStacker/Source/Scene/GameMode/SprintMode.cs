using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

namespace MonoStacker.Source.Scene.GameMode;

public class SprintMode: MarathonMode
{
    public SprintMode()
    {
        _maxLinesCleared = 40;
        _lineGoal = (int)_maxLinesCleared;
        _title = $"{_maxLinesCleared} Lines Sprint";
    }

    public override void Initialize()
    {
        startTimer = 5;
        _currentState = GameState.PreGame;
        _playField = new PlayField(new Vector2(240, 135), PlayFieldPresets.GuidelineFast1, new InputBinds());
        _atSys = new ActionTextSystem(new Vector2(_playField.Offset.X - 13, _playField.Offset.Y + 52));
        _comboCounter = new(-1, 1, .5f, .3f, "Combo *", Color.Orange, new(_playField.Offset.X - 12, _playField.Offset.Y + 41));
        _streakCounter = new(-1, 1, .5f, .3f, "Streak *", Color.Cyan, new(_playField.Offset.X - 12, _playField.Offset.Y + 49));
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
        _level = 1;
        _gravity = SetGravity(_level);
        _playField.gravity = _gravity;
        _levelProgressDisplay = new(new Vector2(_playField.Offset.X - 7, _playField.Offset.Y), _lineGoal, ProgressBarType.Vertical);

        _streakFireSource = new(new(_playField.Offset.X - 48, _playField.Offset.Y + 46));
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
        };
        _streakFireEmitter = new EmitterObj(_streakFireSource, _streakFire, EmissionType.Continuous, false);

        _particleLayer.AddEmitter(_streakFireEmitter);
        _comboFireSource = new(new(_playField.Offset.X - 45, _playField.Offset.Y + 38));
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
        };
        _comboFireEmitter = new EmitterObj(_comboFireSource, _comboFire, EmissionType.Continuous, false);
        _particleLayer.AddEmitter(_comboFireEmitter);

        _levelUpEffectSource = new(new(_playField.Offset.X - 7, _playField.Offset.Y + 160));
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
    }
}