using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj.Tetromino;

namespace MonoStacker.Source.Generic.Rotation.RotationSystems;

public class SuperRotationSys: IRotationSystem
{
    public bool Rotate(Piece piece, Grid grid, RotationType rotationType)
    {
        //currentSpinType = SpinType.None;
        
        int testPt = 0;
        if (piece is not O)
            testPt = (int)SRSData.GetSrsChecks(piece.rotationId, rotationType == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()).Value;
    
        for (int i = 0; i < (piece is I ? SRSData.DataI.GetLength(1) : SRSData.DataJlstz.GetLength(1)); i++)
        {
            if (grid.IsDataPlacementValid(
                    piece.rotations[rotationType == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()],
                    (int)(piece.offsetY -
                          (piece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJlstz[testPt, i].Y)),
                    (int)(piece.offsetX +
                          (piece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJlstz[testPt, i].X))))
            {
                switch (rotationType)
                {
                    case (RotationType)0: piece.RotateCW(); break;
                    case (RotationType)1: piece.RotateCCW(); break;
                }

                piece.offsetX += piece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJlstz[testPt, i].X;
                piece.offsetY -= piece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJlstz[testPt, i].Y;

                /*
                switch (parsedSpins) // since method is bool, just implement check upon if rotation is true in pieceManager class
                {
                    case SpinDenotation.TSpinOnly:
                        if (piece is T)
                            currentSpinType = _grid.CheckForSpin(piece);
                        break;
                    default:
                        currentSpinType = _grid.CheckForSpin(piece);
                        break;
                }
                */


                //if ((int)piece.offsetY == CalculateGhostPiece()) // since method is bool, just implement check upon if rotation is true in pieceManager class
                //ResetLockDelayRotate();
                
                //piece.Update();
                return true;
            }
        }
        return false;
    }
}