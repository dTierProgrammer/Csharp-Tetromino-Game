using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj.Tetromino;

namespace MonoStacker.Source.Generic.Rotation.RotationSystems;

public class SuperRotationSys: IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType)
    {
        var testPt = 0;
        if (piece.type is not TetrominoType.O)
            testPt = SRSData.GetSrsChecks(piece.rotationId, rotationType == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()).Value;
    
        for (int i = 0; i < (piece.type is TetrominoType.I ? SRSData.DataI.GetLength(1) : SRSData.DataJlstz.GetLength(1)); i++)
        {
            if (grid.IsDataPlacementValid(
                    piece.rotations[rotationType == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()],
                    (int)(piece.offsetY -
                          (piece.type is TetrominoType.I ? SRSData.DataI[testPt, i].Y : SRSData.DataJlstz[testPt, i].Y)),
                    (int)(piece.offsetX +
                          (piece.type is TetrominoType.I ? SRSData.DataI[testPt, i].X : SRSData.DataJlstz[testPt, i].X))))
            {
                switch (rotationType)
                {
                    case 0: piece.RotateCW(); break;
                    case (RotationType)1: piece.RotateCCW(); break;
                }

                piece.offsetX += piece.type is TetrominoType.I ? SRSData.DataI[testPt, i].X : SRSData.DataJlstz[testPt, i].X;
                piece.offsetY -= piece.type is TetrominoType.I ? SRSData.DataI[testPt, i].Y : SRSData.DataJlstz[testPt, i].Y;
                return true;
            }
        }
        return false;
    }
}