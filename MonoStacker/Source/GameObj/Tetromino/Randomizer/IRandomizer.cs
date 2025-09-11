using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Randomizer;

public interface IRandomizer
{
    public Piece GetNextTetromino(ITetrominoFactory factory);
    public void SeedRandomizer(int seed);
}