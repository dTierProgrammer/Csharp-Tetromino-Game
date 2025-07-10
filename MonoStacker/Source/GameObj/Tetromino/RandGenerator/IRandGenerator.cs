using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public interface IRandGenerator
{
    public Piece GetNextTetromino(ITetrominoFactory factory);
}