using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.Generic
{
    public class NextPreview
    {
        protected Vector2 _offset;
        protected int queueLength;
        protected static Texture2D borderTexture = GetContent.Load<Texture2D>("Image/Board/queue");
        protected static Texture2D blocks = GetContent.Load<Texture2D>("Image/Block/0q");
        protected static List<Rectangle> queueBorderTiles = new();
        protected static List<Rectangle> queuePieceTiles = new();
        protected const int TILESIZE = 4;
        protected const int GRIDSIZE = 16;
        protected List<Piece> pieceQueue;

        public NextPreview(Vector2 position, int queueLength) 
        {
            _offset = position;
            this.queueLength = queueLength;
            pieceQueue = new();
            GetImageCuts();

            for(int i = 0; i < queueLength; i++)
                pieceQueue.Add(_GenerateTetromino.RandomTetromino7Bag());
        }

        public NextPreview(Vector2 position)
        {
            _offset = position;
            queueLength = 1;
            pieceQueue = new();
            GetImageCuts();
        }

        protected static void GetImageCuts() 
        {
            queuePieceTiles.Add(new Rectangle(0, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 2, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 3, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 4, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 5, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 6, 0, TILESIZE, TILESIZE));
            queuePieceTiles.Add(new Rectangle(TILESIZE * 7, 0, TILESIZE, TILESIZE));

            queueBorderTiles.Add(new Rectangle(0, 0, 16, 16));
            queueBorderTiles.Add(new Rectangle(16, 0, 20, 16));
            queueBorderTiles.Add(new Rectangle(36, 0, 22, 3));
        }

        public Piece GetNextPiece() 
        {
            Piece tetromino = pieceQueue.ElementAt(0);
            pieceQueue.RemoveAt(0);
            return tetromino;
        }

        public virtual void Update() 
        {
            
            if (pieceQueue.Count() < queueLength)
                pieceQueue.Add(_GenerateTetromino.RandomTetromino7Bag());
            
        }

        public virtual void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset)
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
                    if (piece.currentRotation[y, x] != 0) 
                    {
                        spriteBatch.Draw(
                            blocks,
                            new Rectangle((int)(x * TILESIZE + offset.X), (int)(y * TILESIZE + offset.Y), TILESIZE, TILESIZE),
                            sourceRect,
                            Color.White
                            );
                    }
                }
            }
        }

        public void DrawQueue(SpriteBatch spriteBatch) 
        {
            for (int i = 0; i < pieceQueue.Count(); i++)
            {
                DrawPiece(spriteBatch, pieceQueue.ElementAt(i), new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y));
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, _offset.Y - 3), queueBorderTiles[2], Color.White);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, (queueLength * GRIDSIZE) + _offset.Y), queueBorderTiles[2], Color.White);
            for (int i = 0; i < queueLength; i++) 
            {
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[0], Color.White);
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 2, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[1], Color.White);
            }
            DrawQueue(spriteBatch);
        }
    }
}
