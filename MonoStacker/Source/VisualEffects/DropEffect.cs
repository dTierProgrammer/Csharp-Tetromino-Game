using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects;

public class DropEffect: AnimatedEffect
{
    private Texture2D _effect = GetContent.Load<Texture2D>("Image/Effect/dropEffect");
    private Color _tint;
    new Vector2 position;

    private int _rowOffset; // piece y offset
    private int _subRowOffset; // account for empty spaces in piece data (y)
    private int _rowsLength; // dist between column and ghost piece

    private int _columnOffset; // piece x offset
    private int _subColumnOffset; // account for empty spaces in piece data (x)
    private int _columnsLength; // length of piece data (filled)

    private int exOffset;
    private float YDistort = 0;
    private float Buffer;

    private Point _centerPtOffset;

    // remember rows count downwards
    // ts so chopped (not anymore twin)
    public DropEffect(Vector2 position, float timeDisplayed,  Piece piece, int length, Color tint): base(position)
    {
        this.position = position;
        _rowOffset = (int)piece.offsetY;
        _subRowOffset = piece.GetEmptyRows();
        _rowsLength = length;
        _columnOffset = (int)piece.offsetX;
        _subColumnOffset = piece.GetEmptyColumns();
        _columnsLength = piece.GetNonEmptyColumns().Count;
        exOffset = piece.type switch
        {
            GameObj.Tetromino.TetrominoType.I => 1,
            GameObj.Tetromino.TetrominoType.O => 1,
            GameObj.Tetromino.TetrominoType.J => piece.GetLongestRow().row,
            GameObj.Tetromino.TetrominoType.L => piece.GetLongestRow().row,
            _ => piece.GetCenterPtOffset().Y - 1
            
        };
        _tint = tint;
        MaxTimeDisplayed = timeDisplayed;
        TimeDisplayed = timeDisplayed;
    }

    public DropEffect(Vector2 position, float timeDisplayed, Piece piece, int length, Color tint, float YDistort) : base(position)
    {
        this.position = position;
        _rowOffset = (int)piece.offsetY;
        _subRowOffset = piece.GetEmptyRows();
        _rowsLength = length;
        _columnOffset = (int)piece.offsetX;
        _subColumnOffset = piece.GetEmptyColumns();
        _columnsLength = piece.GetNonEmptyColumns().Count;
        exOffset = piece.type switch
        {
            GameObj.Tetromino.TetrominoType.I => 1,
            GameObj.Tetromino.TetrominoType.O => 1,
            GameObj.Tetromino.TetrominoType.J => piece.GetLongestRow().row,
            GameObj.Tetromino.TetrominoType.L => piece.GetLongestRow().row,
            _ => piece.GetCenterPtOffset().Y - 1

        };
        _tint = tint;
        MaxTimeDisplayed = timeDisplayed;
        TimeDisplayed = timeDisplayed;
        this.YDistort = YDistort;
        Buffer = _rowsLength;
    }


    public override void Update(float deltaTime)
    {
        TimeDisplayed -= deltaTime;
        _tint *= (TimeDisplayed / (MaxTimeDisplayed));
        //_rowsLength = (int)MathHelper.Lerp(_rowsLength, _rowsLength + YDistort, .1f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
            (
                _effect,
                new Rectangle
                (
                    (int)(position.X) + (_columnOffset * 8) + (_subColumnOffset * 8),
                    (int)(position.Y) + (_rowOffset * 8) - 160 + (_subRowOffset * 8) + (exOffset * 8),
                    _columnsLength * 8,
                    _rowsLength
                ),
                _tint * .5f
            );
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 drawOffset)
    {
        spriteBatch.Draw
            (
                _effect,
                new Rectangle
                (
                    (int)(position.X) + (_columnOffset * 8) + (_subColumnOffset * 8) + (int)drawOffset.X,
                    (int)(position.Y) + (_rowOffset * 8) - 160 + (_subRowOffset * 8) + (exOffset * 8) + (int)drawOffset.Y,
                    _columnsLength * 8,
                    _rowsLength * 8
                ),
                _tint * .5f
            );
    }
}