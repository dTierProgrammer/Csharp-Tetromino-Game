using System;
using System.Diagnostics;
using MonoStacker.Source.Data;

namespace MonoStacker.Source.Generic.Rotation.RotationSystems;

public class ArcadeRotationSys: IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType)
    {
        for (int i = 0; i < 3; i++)
        {
            if (grid.IsDataPlacementValid(
                    piece.rotations[rotationType == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()],
                    (int)piece.offsetY - ARSData.DataJlostz[0, i].Y,
                    (int)piece.offsetX + ARSData.DataJlostz[0, i].X))
            {
                piece.offsetX += ARSData.DataJlostz[0, i].X;
                //piece.offsetY -= ARSData.DataJlostz[0, i].Y;
                switch (rotationType)
                {
                    case RotationType.Clockwise: piece.RotateCW(); return true;
                    case RotationType.CounterClockwise: piece.RotateCCW(); return true;
                }
                Debug.WriteLine("true");
                
                return true;
            }
        }
        Debug.WriteLine("false");
        return false;
    }
}