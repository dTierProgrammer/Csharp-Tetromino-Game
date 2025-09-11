using System;
using System.Collections.Generic;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class SevenBagRandomizer: IRandomizer
{


    private Random _rng;

    private List<TetrominoType> _bag = [];

    public void SeedRandomizer(int seed)
    {
        //_rng = new(100);
    }

    public SevenBagRandomizer() 
    {
        _rng = new();
    }

    public SevenBagRandomizer(int seed)
    {
        _rng = new(seed);
    }

    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        if (_bag.Count == 0)
            _bag.AddRange(Enum.GetValues<TetrominoType>());

        var nextTetromino = _rng.Next(_bag.Count);
        var piece = factory.NewPiece(_bag[nextTetromino]);
        
        _bag.RemoveAt(nextTetromino);
        return piece;
    }
}