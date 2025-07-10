using System;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public class ArcadeRandGenerator: IRandGenerator
{
    private Random _rng = new ();
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        throw new System.NotImplementedException();
    }
}