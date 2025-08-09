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
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.Global;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoStacker.Source.Generic
{
    public class HoldPreview: NextPreview
    { // deprecated class
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
                    //_playField.activePiece = _playField.nextPreview.GetNextPiece();
                    ResetPiece(HoldBox.ElementAt(0));
                    canHold = false;
                    return true;
                }
                if (HoldBox.Count == QueueLength) 
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

        protected override void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset)
        {
            Rectangle sourceRect = QueuePieceTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    sourceRect = piece.currentRotation[y, x] switch
                    {
                        1 => QueuePieceTiles[0],
                        2 => QueuePieceTiles[1],
                        3 => QueuePieceTiles[2],
                        4 => QueuePieceTiles[3],
                        5 => QueuePieceTiles[4],
                        6 => QueuePieceTiles[5],
                        7 => QueuePieceTiles[6],
                        _ => Rectangle.Empty
                    };
                    if (piece.currentRotation[y, x] > 0)
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((int)(x * Tilesize + offset.X), (int)(y * Tilesize + offset.Y), Tilesize, Tilesize), canHold ? sourceRect : QueuePieceTiles[7], Color.White);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, ITetrominoFactory factory) 
        {
            
            for (var i = 0; i < QueueLength; i++)
            {
                spriteBatch.Draw(BgTexture, new Vector2(Offset.X, (i * Gridsize) + Offset.Y), QueueBgTiles[3], Color.White);
                spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 3, (i * Gridsize) + Offset.Y), QueueBorderTiles[1], Color.White);
            }
            
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 4, Offset.Y - 3), QueueBorderTiles[0], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 4, (QueueLength * Gridsize) + Offset.Y - 7), QueueBorderTiles[2], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 4, Offset.Y - 11), QueueBorderTiles[3], Color.White);
            
            if (HoldBox.Count == 0) return;
            var buffer = HoldBox.ElementAt(0).type switch
            {
                TetrominoType.O => 9,
                TetrominoType.I => 1,
                _ => 5
            };
            
            var bufferY = HoldBox.ElementAt(0).type switch
            {
                TetrominoType.I => 1,
                _ => 5
            };

            var bufferYY = 0;
            if (_playField._pieceData is ArcadeFactory) 
            {
                bufferYY = CheckTopRow(HoldBox.ElementAt(0)) ? 0 : -8;
                if (HoldBox.ElementAt(0).type is TetrominoType.I) bufferYY = 0;
            }
              

            DrawPiece(spriteBatch, HoldBox.ElementAt(0), new Vector2(Offset.X + buffer, Offset.Y + bufferY + bufferYY));
        }
    }
}
