using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Factory;

public interface ITetrominoFactory
{
    public Piece NewPiece(TetrominoType type);
}