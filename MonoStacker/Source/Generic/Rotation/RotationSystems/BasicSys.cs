namespace MonoStacker.Source.Generic.Rotation.RotationSystems;

public class BasicSys: IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType)
    {
        if (grid.IsDataPlacementValid(rotationType == 0 ? piece.rotations[piece.ProjectRotateCW()] : piece.rotations[piece.ProjectRotateCCW()], (int)piece.offsetY, (int)piece.offsetX))
        {
            switch (rotationType)
            {
                case RotationType.Clockwise: piece.RotateCW(); piece.Update(); return true;
                case RotationType.CounterClockwise: piece.RotateCCW(); piece.Update(); return true;
            }
        }
        return false;
    }
}