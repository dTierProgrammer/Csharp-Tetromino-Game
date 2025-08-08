using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class MasterRandomizer : IRandomizer
{ // tgm1 randomizer
    private readonly Random _rng = new();
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

        var piece = factory.NewPiece(nextTetromino);
        piece.offsetX = nextTetromino switch
        {
            TetrominoType.O => 4,
            _ => 3
        };
        piece.initOffsetX = piece.offsetX;

        piece.offsetY = nextTetromino switch
        {
            TetrominoType.O => 16,
            _ => 17
        };
        piece.initOffsetY = piece.offsetY;

        /*
        foreach (var item in _tetrominoHistory)
            Console.Write(item);
        Console.WriteLine();
        */

        _totalRolls++;
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