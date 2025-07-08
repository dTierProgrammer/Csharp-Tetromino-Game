namespace MonoStacker.Source.Generic.Rotation.RotationSystems;

public class ArcadeRotationSys: IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType)
    {
        //throw new System.NotImplementedException();
        piece.Update();
        return false;
    }
}