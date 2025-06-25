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
        protected static Texture2D borderTexture = GetContent.Load<Texture2D>("Image/Board/queue_q");
        protected static Texture2D blocks = GetContent.Load<Texture2D>("Image/Block/1");
        protected static List<Rectangle> queueBorderTiles = new();
        protected static List<Rectangle> queuePieceTiles = new();
        protected const int TILESIZE = 8;
        protected const int GRIDSIZE = 24;
        protected List<Piece> pieceQueue;

        bool alignPiecesToCenter = true;

        public NextPreview(Vector2 position, int queueLength) 
        {
            _offset = position;
            this.queueLength = queueLength;
            pieceQueue = new();
            GetImageCuts();

            for(int i = 0; i < 7; i++)
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
            queuePieceTiles.Add(new Rectangle(0, 0, TILESIZE, TILESIZE)); // I
            queuePieceTiles.Add(new Rectangle(TILESIZE, 0, TILESIZE, TILESIZE)); // J
            queuePieceTiles.Add(new Rectangle(TILESIZE * 2, 0, TILESIZE, TILESIZE)); // L
            queuePieceTiles.Add(new Rectangle(TILESIZE * 3, 0, TILESIZE, TILESIZE)); // O
            queuePieceTiles.Add(new Rectangle(TILESIZE * 4, 0, TILESIZE, TILESIZE)); // S
            queuePieceTiles.Add(new Rectangle(TILESIZE * 5, 0, TILESIZE, TILESIZE)); // T
            queuePieceTiles.Add(new Rectangle(TILESIZE * 6, 0, TILESIZE, TILESIZE)); // Z
            queuePieceTiles.Add(new Rectangle(0, 8, 8, 8));

            queueBorderTiles.Add(new Rectangle(0, 0, 40, 12)); // top
            queueBorderTiles.Add(new Rectangle(0, 12, 40, GRIDSIZE)); // sides
            queueBorderTiles.Add(new Rectangle(0, 44, 40, 4)); // bottom
            queueBorderTiles.Add(new Rectangle(40, 12, 32, GRIDSIZE)); // grid
            

            queueBorderTiles.Add(new Rectangle(40, 0, 41, 12)); // top hold
        }

        public Piece GetNextPiece() 
        {
            Piece tetromino = pieceQueue.ElementAt(0);
            pieceQueue.RemoveAt(0);
            return tetromino;
        }

        public virtual void Update() 
        {
            
            if (pieceQueue.Count() < 7)
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
                    if (piece.currentRotation[y, x] > 0) 
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
            int buffer = 0;
            for (int i = 1; i <= 7; i++)
            {
                
                if ((i - 1) < queueLength) 
                {
                    if (pieceQueue.ElementAt(i - 1) is O)
                        buffer = 8;
                    else if (pieceQueue.ElementAt(i - 1) is I)
                        buffer = 0;
                    else
                        buffer = 4;

                        DrawPiece(spriteBatch, pieceQueue.ElementAt(i - 1), new Vector2(_offset.X + buffer, ((i - 1) * GRIDSIZE) + _offset.Y));
                }
                    
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            /*
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, _offset.Y - 3), queueBorderTiles[0], Color.White);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, (queueLength * GRIDSIZE) + _offset.Y), queueBorderTiles[2], Color.White);
            for (int i = 0; i < queueLength; i++) 
            {
                //spriteBatch.Draw(borderTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[0], Color.White);
                //spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 2, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[3], Color.White);
            }
            */
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, _offset.Y - 12), queueBorderTiles[0], Color.White);
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, (queueLength * GRIDSIZE) + _offset.Y), queueBorderTiles[2], Color.White);
            for (int i = 0; i < queueLength; i++)
            {
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 4, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[1], Color.White);
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[3], Color.White);
            }
            DrawQueue(spriteBatch);
        }
    }
}
