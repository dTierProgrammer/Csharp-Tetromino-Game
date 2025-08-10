using System;
using System.Collections.Generic;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public class SevenBagRandomizer: IRandomizer
{
    private readonly Random _rng = new ();

    private List<TetrominoType> _bag = [];
    
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