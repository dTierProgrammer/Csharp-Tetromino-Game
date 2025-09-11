using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using System;
using System.Diagnostics;

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
        _playField = new PlayField(new Vector2(240, 135),new SevenBagRandomizer(), PlayFieldPresets.GuidelineFast1, InputDevice.Keyboard, null, new InputBinds());
        _atSys = new ActionTextSystem(new Vector2(_playField.Offset.X - 13, _playField.Offset.Y + 52) - new Vector2(_playField.fixOffset.X / 2, _playField.fixOffset.Y / 4));
        _comboCounter = new(-1, 1, .5f, .3f, "Combo *", Color.Orange, new Vector2 (_playField.Offset.X - 12, _playField.Offset.Y + 41) - new Vector2(_playField.fixOffset.X / 2, _playField.fixOffset.Y / 4));
        _streakCounter = new(-1, 1, .5f, .3f, "Streak *", Color.Cyan, new Vector2(_playField.Offset.X - 12, _playField.Offset.Y + 49) - new Vector2(_playField.fixOffset.X / 2, _playField.fixOffset.Y / 4));
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

        InitEffects();
        Debug.WriteLine($"Sprint | {TimeSpan.FromSeconds(Game1.uGameTime.TotalGameTime.TotalSeconds).ToString(@"mm\:ss\.ff")} | Initialization success, seed: {seed}.");
    }
}