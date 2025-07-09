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
        protected static Texture2D borderTexture = GetContent.Load<Texture2D>("Image/Board/queue0");
        protected static Texture2D bgTexture = GetContent.Load<Texture2D>("Image/Board/queue_bg");
        protected static Texture2D blocks = GetContent.Load<Texture2D>("Image/Block/1");
        protected static List<Rectangle> queueBorderTiles = new();
        protected static List<Rectangle> queuePieceTiles = new();
        protected static List<Rectangle> queueBgTiles = new();
        protected const int TILESIZE = 8;
        protected const int GRIDSIZE = 25;
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

            queueBorderTiles.Add(new Rectangle(0, 0, 41, 11)); // top (0)
            queueBorderTiles.Add(new Rectangle(0, 12, 40, 25)); // sides (1)
            queueBorderTiles.Add(new Rectangle(0, 38, 41, 11)); // bottom (2)
            queueBorderTiles.Add(new Rectangle(0, 50, 22, 7)); // hold title (3)
            queueBorderTiles.Add(new Rectangle(0, 58, 21, 7)); // next title (4)
            
            queueBgTiles.Add(new Rectangle(0, 0, 34, 25)); // bg top (0)
            queueBgTiles.Add(new Rectangle(0, 26, 34, 25)); // bg mid (1)
            queueBgTiles.Add(new Rectangle(0, 52, 34, 25)); // bg bottom (2)
            queueBgTiles.Add(new Rectangle(0, 78, 34, 25)); // bg static (3)
            

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
            //Rectangle sourceRect = queuePieceTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++) 
                {
                    var sourceRect = piece.currentRotation[y, x] switch
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
                    {
                        spriteBatch.Draw(
                            ImgBank.BlockTexture,
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
            //int buffer = 0;
            for (int i = 1; i <= 7; i++)
            {
                if ((i - 1) < queueLength) 
                {
                    var buffer = pieceQueue.ElementAt(i - 1) switch
                    {
                        O => 9,
                        I => 1,
                        _ => 5
                    };
                    
                    var bufferY = pieceQueue.ElementAt(i - 1) switch
                    {
                        I => 1,
                        _ => 5
                    };
                    
                    DrawPiece(spriteBatch, pieceQueue.ElementAt(i - 1), new Vector2(_offset.X + buffer, ((i - 1) * GRIDSIZE) + _offset.Y + bufferY));
                }
                    
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            for (int i = 0; i < queueLength; i++)
            {
                spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, (i * GRIDSIZE) + _offset.Y), queueBorderTiles[1], Color.White);
                spriteBatch.Draw(bgTexture, new Vector2(_offset.X, (i * GRIDSIZE) + _offset.Y), queueBgTiles[3], Color.White);
            }
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, _offset.Y - 3), queueBorderTiles[0], Color.White); // top
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3, (queueLength * GRIDSIZE) + _offset.Y - 7), queueBorderTiles[2], Color.White); // bottom
            spriteBatch.Draw(borderTexture, new Vector2(_offset.X - 3 , _offset.Y - 11), queueBorderTiles[4], Color.White);
            DrawQueue(spriteBatch);
        }
    }
}
