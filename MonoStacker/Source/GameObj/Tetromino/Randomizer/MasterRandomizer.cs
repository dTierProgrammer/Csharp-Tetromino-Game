using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class MasterRandomizer : IRandomizer
{ // tgm1 randomizer
    

    private Random _rng = new();
    private readonly Queue<TetrominoType> _tetrominoHistory = [];
    private const int EntryLimit = 4;
    private readonly Array _tetrominos = Enum.GetValues<TetrominoType>();
    private readonly TetrominoType[] _initTetrominos = // for initial roll
    {
        TetrominoType.I,
        TetrominoType.J,
        TetrominoType.L,
        TetrominoType.T,
    };
    private int _totalRolls = 0;

    public MasterRandomizer()
    {
        _rng = new();
    }

    public MasterRandomizer(int seed)
    {
        _rng = new(seed);
    }

    public void SeedRandomizer(int seed) 
    {
        _rng = new(seed);
    }
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        if (!_tetrominoHistory.Any())
        {
            for (int i = 1; i < EntryLimit; i++)
            {
                _tetrominoHistory.Enqueue(TetrominoType.Z);
            }
        }

        var nextTetromino = TetrominoType.I;
        if (_totalRolls == 0)
            nextTetromino = _initTetrominos[_rng.Next(0, 3)];
        else
        {
            for (var i = 0; i < 4; i++)
            {
                nextTetromino = (TetrominoType)_rng.Next(0, 7);
                if (!CheckHistory(nextTetromino) || i == 3) break;
            }
        }
        AddToHistory(nextTetromino);

        _totalRolls++;
        return factory.NewPiece(nextTetromino);
    }

    private void AddToHistory(TetrominoType tetromino)
    {
        if (_tetrominoHistory.Count < EntryLimit)
            _tetrominoHistory.Enqueue(tetromino);
        else
        {
            _tetrominoHistory.Dequeue();
            _tetrominoHistory.Enqueue(tetromino);
        }
    }

    private bool CheckHistory(TetrominoType tetromino)
    {
        foreach (var item in _tetrominoHistory)
        {
            if (tetromino == item)
                return true;
        }
        return false;
    }
}