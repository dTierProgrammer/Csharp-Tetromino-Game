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
    private Piece piece;
    private Color _tint;

    private int _rowOffset; // piece y offset
    private int _subRowOffset; // account for empty spaces in piece data (y)
    private float _rowsLength; // dist between column and ghost piece

    private int _columnOffset; // piece x offset
    private int _subColumnOffset; // account for empty spaces in piece data (x)
    private int _columnsLength; // length of piece data (filled)

    // remember rows count downwards
    // ts so chopped
    public DropEffect(Vector2 position, float timeDisplayed,  Piece piece, int length, Color tint): base(position)
    {
        _rowOffset = (int)piece.offsetY;
        _subRowOffset = piece.GetEmptyRows();
        _rowsLength = length + 1;
        _columnOffset = (int)piece.offsetX;
        _subColumnOffset = piece.GetEmptyColumns();
        _columnsLength = piece.GetNonEmptyColumns();
        _tint = tint;
        MaxTimeDisplayed = timeDisplayed;
        TimeDisplayed = timeDisplayed;
    }

   
    public override void Update(float deltaTime)
    {
        TimeDisplayed -= deltaTime;
        _tint *= (TimeDisplayed / (MaxTimeDisplayed));
        //Debug.WriteLine(_rowsLength);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
            (
                _effect,
                new Rectangle
                (
                    (int)(position.X) + (_columnOffset * 8) + (_subColumnOffset * 8),
                    (int)(position.Y) + (_rowOffset * 8) - 160 + (_subRowOffset * 8),
                    _columnsLength * 8,
                    (int)_rowsLength * 8
                ),
                _tint * .5f
            );
    }
}