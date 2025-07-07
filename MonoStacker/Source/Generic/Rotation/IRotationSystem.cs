using MonoStacker.Source.GameObj;

namespace MonoStacker.Source.Generic.Rotation;

public interface IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType);
}