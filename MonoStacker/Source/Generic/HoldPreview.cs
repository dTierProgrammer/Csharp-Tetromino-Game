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
        private SoundEffect holdPiece = GetContent.Load<SoundEffect>("Audio/Sound/hold");

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
            piece.rotationId = 0;
            piece.Update();
        }

        public void SwapPiece()
        {
            if (canHold) 
            {
                if (HoldBox.Count == 0)
                {
                    HoldBox.Add(_playField.activePiece);
                    _playField.activePiece = _playField.nextPreview.GetNextPiece();
                    ResetPiece(HoldBox.ElementAt(0));
                    canHold = false;
                }
                else if (HoldBox.Count == queueLength) 
                {
                    Piece prevActivePiece = _playField.activePiece ;

                    _playField.activePiece = HoldBox.ElementAt(0);
                    HoldBox.RemoveAt(0);
                    HoldBox.Add(prevActivePiece);
                    ResetPiece(HoldBox.ElementAt(0));
                    canHold = false;
                }
                holdPiece.Play();
    }
        }

        public override void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset)
        {
            Rectangle sourceRect = queuePieceTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    switch (piece.currentRotation[y, x])
                    {
                        case 1:
                            sourceRect = queuePieceTiles[0];
                            break;
                        case 2:
                            sourceRect = queuePieceTiles[1];
                            break;
                        case 3:
                            sourceRect = queuePieceTiles[2];
                            break;
                        case 4:
                            sourceRect = queuePieceTiles[3];
                            break;
                        case 5:
                            sourceRect = queuePieceTiles[4];
                            break;
                        case 6:
                            sourceRect = queuePieceTiles[5];
                            break;
                        case 7:
                            sourceRect = queuePieceTiles[6];
                            break;
                    }
                    if (piece.currentRotation[y, x] > 0)
                    {
                        spriteBatch.Draw(
                            blocks,
                            new Rectangle((int)(x * TILESIZE + offset.X), (int)(y * TILESIZE + offset.Y), TILESIZE, TILESIZE),
                            canHold ? sourceRect : queuePieceTiles[7],
                            Color.White
                            );
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, _offset.Y - 12), queueBorderTiles[4], Color.White);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, (queueLength * GRIDSIZE) + _offset.Y), queueBorderTiles[2], Color.White);
            for (int i = 0; i < queueLength; i++)
            {
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[1], Color.White);
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[3], Color.White);
            }
            if (HoldBox.Count() > 0) 
            {
                int buffer = 0;
                if (HoldBox.ElementAt(0) is O)
                    buffer = 8;
                else if (HoldBox.ElementAt(0) is I)
                    buffer = 0;
                else
                    buffer = 4;
                DrawPiece(spriteBatch, HoldBox.ElementAt(0), new Vector2(_offset.X + buffer, _offset.Y));
            }
                
        }
    }
}
