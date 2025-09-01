using Microsoft.Xna.Framework;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;

namespace MonoStacker.Source.Scene.GameMode;

public class SprintMode: MarathonMode
{
    public SprintMode()
    {
        _maxLinesCleared = 40;
        _lineGoal = 40;
        _title = $"{_maxLinesCleared} Lines Sprint";
    }

    public override void Initialize()
    {
        startTimer = 5;
        _currentState = GameState.PreGame;
        _playField = new PlayField(new Vector2(240, 135), new PlayFieldData(){lineClearDelay = 0, arrivalDelay = 0, autoshiftRepeatRate = 20, softDropFactor = 10000}, new InputBinds());
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
}