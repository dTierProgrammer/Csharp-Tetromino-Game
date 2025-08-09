using Microsoft.Xna.Framework;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Factory;

public interface ITetrominoFactory
{
    public Piece NewPiece(TetrominoType type);
    public int[][] SpawnArea();
    public Point SpawnOffset_Jlstz();
    public Point SpawnOffset_I();
    public Point SpawnOffset_O();
}