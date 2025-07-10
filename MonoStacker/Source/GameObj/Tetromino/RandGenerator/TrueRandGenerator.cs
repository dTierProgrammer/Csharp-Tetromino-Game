using System;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public class TrueRandGenerator: IRandGenerator
{
    private readonly Random _rng = new ();
    public Piece GetNextTetromino(ITetrominoFactory factory)
    {
        Array tetrominos = Enum.GetValues<TetrominoType>();
        var nextTetromino = (TetrominoType)_rng.Next(0, tetrominos.Length);
        var piece = factory.NewPiece(nextTetromino);
        
        piece.offsetX = nextTetromino switch
        {
            TetrominoType.O => 4,
            _ => 3
        };
        piece.initOffsetX = piece.offsetX;
        
        piece.offsetY = nextTetromino switch
        {
            TetrominoType.O => 17,
            _ => 18
        };
        piece.initOffsetY = piece.offsetY;
        
        return piece;
    }
}