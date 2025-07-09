using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Global;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoStacker.Source.Generic
{
    public class HoldPreview: NextPreview
    {
        PlayField _playField;
        public bool canHold;
        List<Piece> HoldBox;

        public HoldPreview(Vector2 position, PlayField playField) : base(position) 
        {
            _playField = playField;
            canHold = true;
            HoldBox = new();
        }

        public void ResetPiece(Piece piece) 
        {
            piece.offsetX = piece.initOffsetX;
            piece.offsetY = piece.initOffsetY;
            piece.ResetId();
            piece.Update();
        }

        public bool ChangePiece()
        {
            if (canHold) 
            {
                if (HoldBox.Count == 0)
                {
                    HoldBox.Add(_playField.activePiece);
                    _playField.activePiece = _playField.nextPreview.GetNextPiece();
                    ResetPiece(HoldBox.ElementAt(0));
                    canHold = false;
                    return true;
                }
                if (HoldBox.Count == queueLength) 
                {
                    Piece prevActivePiece = _playField.activePiece ;

                    _playField.activePiece = HoldBox.ElementAt(0);
                    HoldBox.RemoveAt(0);
                    HoldBox.Add(prevActivePiece);
                    ResetPiece(HoldBox.ElementAt(0));
                    canHold = false;
                    return true;
                }
            }
            return false;
        }

        public override void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset)
        {
            Rectangle sourceRect = queuePieceTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    sourceRect = piece.currentRotation[y, x] switch
                    {
                        1 => queuePieceTiles[0],
                        2 => queuePieceTiles[1],
                        3 => queuePieceTiles[2],
                        4 => queuePieceTiles[3],
                        5 => queuePieceTiles[4],
                        6 => queuePieceTiles[5],
                        7 => queuePieceTiles[6],
                        _ => Rectangle.Empty
                    };
                    if (piece.currentRotation[y, x] > 0)
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((int)(x * TILESIZE + offset.X), (int)(y * TILESIZE + offset.Y), TILESIZE, TILESIZE), canHold ? sourceRect : queuePieceTiles[7], Color.White);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            
            for (var i = 0; i < queueLength; i++)
            {
                spriteBatch.Draw(bgTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBgTiles[3], Color.White);
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[1], Color.White);
            }
            
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, _offset.Y - 3), queueBorderTiles[0], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, (queueLength * GRIDSIZE) + _offset.Y - 7), queueBorderTiles[2], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, _offset.Y - 11), queueBorderTiles[3], Color.White);
            
            if (HoldBox.Count == 0) return;
            var buffer = HoldBox.ElementAt(0) switch
            {
                O => 9,
                I => 1,
                _ => 5
            };
            
            var bufferY = HoldBox.ElementAt(0) switch
            {
                I => 1,
                _ => 5
            };
            
            DrawPiece(spriteBatch, HoldBox.ElementAt(0), new Vector2(_offset.X + buffer, _offset.Y + bufferY));
        }
    }
}
