using System;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class UnbiasedRandomizer: IRandomizer
{
    private readonly Random _rng = new ();
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        Array tetrominos = Enum.GetValues<TetrominoType>();
        var nextTetromino = (TetrominoType)_rng.Next(0, tetrominos.Length);
        
        return factory.NewPiece(nextTetromino);
    }
}