using System;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class UnbiasedRandomizer: IRandomizer
{
    private Random _rng = new ();

    public void SeedRandomizer(int seed)
    {
        _rng = new(seed);
    }

    public UnbiasedRandomizer()
    {
        _rng = new();
    }

    public UnbiasedRandomizer(int seed)
    {
        _rng = new(seed);
    }
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        Array tetrominos = Enum.GetValues<TetrominoType>();
        var nextTetromino = (TetrominoType)_rng.Next(0, tetrominos.Length);

        var piece = factory.NewPiece(nextTetromino);
        return piece;
    }
}