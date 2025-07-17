using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public class MasterRandomizer: IRandGenerator
{ // tgm1 randomizer
    private readonly Random _rng = new ();
    private readonly Queue<TetrominoType> _tetrominoHistory = [];
    private const int EntryLimit = 4;
   
    private readonly Array _tetrominos = Enum.GetValues<TetrominoType>();
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        var nextTetromino = (TetrominoType)_rng.Next(0, _tetrominos.Length);
        if (_tetrominoHistory.Count == 0) 
        {
            for (int i = 0; i < EntryLimit; i++)
                _tetrominoHistory.Enqueue(TetrominoType.Z);
        }
        if (_tetrominoHistory.Any())
        {
            if (CheckHistory(nextTetromino))
            {
                var tries = 4;
                for (var i = 0; i < tries; i++)
                {
                    nextTetromino = (TetrominoType)_rng.Next(0, _tetrominos.Length);
                    if (!CheckHistory(nextTetromino)) break;
                }
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
            TetrominoType.I => 17,
            _ => 17
        };
        piece.initOffsetY = piece.offsetY;
        //Console.WriteLine("****");
        foreach (var item in _tetrominoHistory)
        {
            Console.Write(item);
        }

        Console.WriteLine();
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
            if (tetromino == item);
            return true;
        }
        return false;
    }
}