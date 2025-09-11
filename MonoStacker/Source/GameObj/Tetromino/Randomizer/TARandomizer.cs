using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class TARandomizer: IRandomizer
{ // tap randomizer (same as tgm1, but 6 rerolls)
    private Random _rng = new();
    private readonly Queue<TetrominoType> _tetrominoHistory = [];
    private const int EntryLimit = 4;
    private readonly TetrominoType[] _initTetrominos = // for initial roll
    {
        TetrominoType.I,
        TetrominoType.J,
        TetrominoType.L,
        TetrominoType.T,
    };
    private int _totalRolls = 0;

    public TARandomizer()
    {
        _rng = new();
    }

    public TARandomizer(int seed)
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
                if (i <= 2) _tetrominoHistory.Enqueue(TetrominoType.Z);
                else _tetrominoHistory.Enqueue(TetrominoType.S);
            }
        }

        var nextTetromino = TetrominoType.I;
        if (_totalRolls == 0)
            nextTetromino = _initTetrominos[_rng.Next(0, 3)];
        else 
        {
            for (var i = 0; i < 6; i++)
            {
                nextTetromino = (TetrominoType)_rng.Next(0, 7);
                if (!CheckHistory(nextTetromino) || i == 5) break;
            }
        }
        AddToHistory(nextTetromino);

        _totalRolls++;
        var piece = factory.NewPiece(nextTetromino);
        return piece;
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