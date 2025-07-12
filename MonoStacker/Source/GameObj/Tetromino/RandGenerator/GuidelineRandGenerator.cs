using System;
using System.Collections.Generic;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public class GuidelineRandGenerator: IRandGenerator
{
    private readonly Random _rng = new ();

    private List<TetrominoType> _bag = [];
    
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        if (_bag.Count == 0)
        {
            _bag.Add(TetrominoType.I);
            _bag.Add(TetrominoType.J);
            _bag.Add(TetrominoType.L);
            _bag.Add(TetrominoType.O);
            _bag.Add(TetrominoType.S);
            _bag.Add(TetrominoType.T);
            _bag.Add(TetrominoType.Z);
        }

        var nextTetromino = _rng.Next(_bag.Count);
        var piece = factory.NewPiece(_bag[nextTetromino]);
        
        piece.offsetX = _bag[nextTetromino] switch
        {
            TetrominoType.O => 4,
            _ => 3
        };
        piece.initOffsetX = piece.offsetX;
        
        piece.offsetY = _bag[nextTetromino] switch
        {
            TetrominoType.I => 17,
            _ => 18
        };

        piece.initOffsetY = piece.offsetY;
        _bag.RemoveAt(nextTetromino);
        return piece;
    }
}