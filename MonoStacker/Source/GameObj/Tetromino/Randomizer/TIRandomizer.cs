using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class TIRandomizer: IRandomizer
{ // tgm3 randomizer

    /*
    Initialize:
    - 35 pc. pool, 5 of every piece
    - First piece must be I, J, L, T
    - Initialize history to [S, Z, S, first piece drawn] (use queue)
    
    Function:
    - Get piece from bag, reroll >= 6 times if history contains piece
    - Pop first piece (driest piece) into pool every grab
    - Return piece
    */

    private readonly Random _rng = new Random();
    private readonly TetrominoType[] _initTetrominos = // for initial roll
    { 
        TetrominoType.I,
        TetrominoType.J,
        TetrominoType.L,
        TetrominoType.T,
    };
    private readonly List<TetrominoType> _pool = new();
    private readonly Queue<TetrominoType> _history = new();
    private int entryLimit = 4;
    private int _totalRolls = 0;

    public Piece GetNextTetromino(ITetrominoFactory factory) 
    {
        if (!_pool.Any())
        {
            for (var i = 0; i < 5; i++)
                _pool.AddRange(Enum.GetValues<TetrominoType>());
        }

        if (!_history.Any()) 
        {
            for (var i = 1; i <= 3; i++) 
            {
                if (i % 2 != 0) _history.Enqueue(TetrominoType.Z);
                else _history.Enqueue(TetrominoType.S);
            }
        }

        var nextTetromino = TetrominoType.I;
        if (_totalRolls == 0)
            nextTetromino = _initTetrominos[_rng.Next(0, 3)];
        else 
        {
            for (var i = 0; i < 6; i++)
            {
                nextTetromino = _pool[_rng.Next(35)];
                if (!CheckHistory(nextTetromino) || i == 5) break;
            }
        }
        AddToHistory(nextTetromino);

        _totalRolls++;
        return factory.NewPiece(nextTetromino);
    }

    private void AddToHistory(TetrominoType tetromino) 
    {
        if (_history.Count < entryLimit)
            _history.Enqueue(tetromino);
        else 
        {
            _pool.Add(_history.Dequeue());
            _history.Enqueue(tetromino);
        }
    }

    private bool CheckHistory(TetrominoType tetromino) 
    {
        foreach (var item in _history) 
        {
            if (tetromino == item)
                return true;
        }
        return false;
    }
}