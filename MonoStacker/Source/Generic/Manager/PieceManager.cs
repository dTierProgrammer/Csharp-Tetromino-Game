using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface.Input;

namespace MonoStacker.Source.Generic.Manager;

public class PieceManager(
    Vector2 offset,
    Piece piece,
    Grid grid,
    IRotationSystem rotationSystem,
    float lockDelay,
    int stepResets,
    int rotateResets,
    float gravity
)
{
    /*
     Handle active piece manipulation, updates, drawing
     TODO: move piece management functions, input manager from playfield class to here, extend with planned rotation module sys
     */
    
    private float _lockDelayLeft = lockDelay;
    private int _stepResetsLeft = stepResets;
    private int _rotateResetsLeft = rotateResets;
    public bool softDrop { get; set; }
    private Color _color;
    public bool showGhostPiece { get; set; }

    public bool Rotate(RotationType rotationType)
    {
        if (rotationSystem.Rotate(piece, grid, rotationType))
            return true;
        return false;
    }

    private int GhostPieceLocation()
    {
        int xOff = (int)piece.offsetX;
        int yOff = (int)piece.offsetY;
        while (grid.IsPlacementValid(piece, yOff + 1, xOff))
            yOff++;
        return yOff;
    }

    public bool MovePiece(float movementAmt)
    {
        if (grid.IsPlacementValid(piece, (int)piece.offsetY, (int)(piece.offsetX + movementAmt)))
        {
            piece.offsetX += movementAmt;
            //SfxBank.stepHori.Play();
            if ((int)piece.offsetY == (int)GhostPieceLocation())
                StepReset();
            return true;
        }

        return false;
    }

    private bool CanDas(GameTime gameTime, float timeStamp)
    {
        //if ((float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= maxDasTime) 
        //return true;
        return false;
    }

    public bool LockPiece()
    {
        _lockDelayLeft = lockDelay;
        _stepResetsLeft = stepResets;
        _rotateResetsLeft = rotateResets;
        grid.LockPiece(piece, (int)piece.offsetY, (int)piece.offsetX);
        return true;
    }

    private bool StepReset()
    {
        if (_stepResetsLeft > 0)
        {
            _lockDelayLeft = lockDelay;
            _stepResetsLeft--;
            return true;
        }

        return false;
    }

    private bool RotateReset()
    {
        if (_rotateResetsLeft > 0)
        {
            _lockDelayLeft = lockDelay;
            _rotateResetsLeft--;
            return true;
        }

        return false;
    }

    private void DropPiece(GameTime gameTime)
    {
        if (piece.offsetY + gravity <= GhostPieceLocation())
        {
            if (grid.IsPlacementValid(piece, (int)(piece.offsetY + gravity), (int)piece.offsetX))
                piece.offsetY += gravity;
        }
        else
            piece.offsetY = GhostPieceLocation();


        if ((int)piece.offsetY == GhostPieceLocation())
        {
            _lockDelayLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _color = Color.Lerp(new(90, 90, 90), Color.White, (MathHelper.Clamp(_lockDelayLeft / lockDelay, 0, 1)));
        }
        else
            _color = Color.White;
    }

    private void HardDrop()
    {
        piece.offsetY = GhostPieceLocation();
        LockPiece();
    }

    private void FirmDrop()
    {
        piece.offsetY = GhostPieceLocation();
    }

    public void DrawPiece(SpriteBatch spriteBatch)
    {
        Rectangle sourceRect = grid.imageTiles[0];
        for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
        {
            for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
            {
                switch (piece.currentRotation[y, x])
                {
                    case 1: sourceRect = grid.imageTiles[0]; break;
                    case 2: sourceRect = grid.imageTiles[1]; break;
                    case 3: sourceRect = grid.imageTiles[2]; break;
                    case 4: sourceRect = grid.imageTiles[3]; break;
                    case 5: sourceRect = grid.imageTiles[4]; break;
                    case 6: sourceRect = grid.imageTiles[5]; break;
                    case 7: sourceRect = grid.imageTiles[6]; break;
                }
                if (piece.currentRotation[y, x] > 0) 
                {
                    if (showGhostPiece)
                    {
                        spriteBatch.Draw(
                            grid.ghostBlocks,
                            new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (GhostPieceLocation() * 8) + (int)offset.Y - 160, 8, 8),
                            sourceRect,
                            Color.White * .5f
                        );
                    }
                    
                    spriteBatch.Draw(
                        grid.blocks,
                        new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8),
                        sourceRect,
                        _color
                    );
                }
            }
        }
    }
}