using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects;

public class DropEffect: AnimatedEffect
{
    private Texture2D _effect = GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect");
    private Piece _piece; // grab offset values from here
    private float _distortFactor;
    private float _rectWidth;
    private int widthBuffer;
    private int _rectHeight;
    private int _offsetX;
    private int _startOffsetY;
    private int _endOffset;
    Color _tint = Color.White;

    /*
     - iterate column by colum through piece's current rotation
        - break loop whenever filled data cell is reached, iterate counter (get width of effect)
        - another time, break at first filled cell (get offset of effect)
     - difference of endOffset and pieceY offset is the height of effect
     - multiply everything by 8 to get sizes/distances in actual pixels
     */

    // ts so chopped
    public DropEffect(Vector2 position, Piece piece, int endOffsetY, float timeDisplayed, Color tint): base(position)
    {
        _piece = piece;
        _endOffset = endOffsetY;
        _distortFactor = 0;
        TimeDisplayed = timeDisplayed;
        MaxTimeDisplayed = timeDisplayed;
        _tint = tint;
        _rectWidth = GetWidthFromData();
        widthBuffer = GetWidthFromData();
        _rectHeight = 50; //(int)((_endOffset - _piece.offsetY) * 8);
    }

    private int GetWidthFromData()
    {
        int units = 0;
        for (var x = 0; x < _piece.currentRotation.GetLength(1); x++) // row
        {
            for (var y = 0; y < _piece.currentRotation.GetLength(0); y++)
            {
                if (_piece.currentRotation[y, x] != 0)
                {
                    units++;
                    break;
                }
            }
        }

        return units * 8;
    }

    private Point GetPlacementFromData()
    {
        for (var x = 0; x < _piece.currentRotation.GetLength(1); x++) // row
        {
            for (var y = 0; y < _piece.currentRotation.GetLength(0); y++)
            {
                if (_piece.currentRotation[y, x] != 0)
                {
                    return new Point(x * 8, y * 8);
                }
            }
        } 
        return new Point(0, 0);
    }

    public override void Update(float deltaTime)
    {
        if (TimeDisplayed == MaxTimeDisplayed)
            Console.WriteLine((_endOffset - (int)_piece.offsetY * 8));
        TimeDisplayed -= deltaTime;
        _rectWidth += (int)_distortFactor;
        _tint *= TimeDisplayed / MaxTimeDisplayed;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
        (
            _effect,
            new Rectangle
            (
                (_piece.currentRotation.GetLength(1) * 8 ) / 2 + (int)position.X,
                (int)(_piece.offsetY * 8) + (int)position.Y - 160,
                (int)_rectWidth, // get width
                (_endOffset - (int)_piece.offsetY * 8) * -1
            ),
            null,
            _tint,
            0,
            new Vector2(0 ,0),
            SpriteEffects.None,
            1
        );
    }
}