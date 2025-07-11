using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.RandGenerator;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.Generic
{
    public class NextPreview
    {
        protected Vector2 Offset;
        protected readonly int QueueLength;
        protected static readonly Texture2D BorderTexture = GetContent.Load<Texture2D>("Image/Board/queue0");
        protected static readonly Texture2D BgTexture = GetContent.Load<Texture2D>("Image/Board/queue_bg");
        protected static readonly List<Rectangle> QueueBorderTiles = [];
        protected static readonly List<Rectangle> QueuePieceTiles = [];
        protected static readonly List<Rectangle> QueueBgTiles = [];
        protected const int Tilesize = 8;
        protected const int Gridsize = 25;
        private readonly Queue<Piece> _pieceQueue;
        private readonly ITetrominoFactory _factory;
        private readonly IRandGenerator _generator;

        public NextPreview(Vector2 position, int queueLength, ITetrominoFactory factory, IRandGenerator generator) 
        {
            Offset = position;
            QueueLength = queueLength;
            _pieceQueue = [];
            _factory = factory;
            _generator = generator;
            GetImageCuts();

            for(var i = 0; i < 7; i++)
                _pieceQueue.Enqueue(_generator.GetNextTetromino(_factory));
        }

        protected NextPreview(Vector2 position)
        {
            Offset = position;
            QueueLength = 1;
            _pieceQueue = [];
            GetImageCuts();
        }

        private static void GetImageCuts() 
        {
            QueuePieceTiles.Add(new Rectangle(0, 0, Tilesize, Tilesize)); // I
            QueuePieceTiles.Add(new Rectangle(Tilesize, 0, Tilesize, Tilesize)); // J
            QueuePieceTiles.Add(new Rectangle(Tilesize * 2, 0, Tilesize, Tilesize)); // L
            QueuePieceTiles.Add(new Rectangle(Tilesize * 3, 0, Tilesize, Tilesize)); // O
            QueuePieceTiles.Add(new Rectangle(Tilesize * 4, 0, Tilesize, Tilesize)); // S
            QueuePieceTiles.Add(new Rectangle(Tilesize * 5, 0, Tilesize, Tilesize)); // T
            QueuePieceTiles.Add(new Rectangle(Tilesize * 6, 0, Tilesize, Tilesize)); // Z
            QueuePieceTiles.Add(new Rectangle(0, 8, 8, 8));

            QueueBorderTiles.Add(new Rectangle(0, 0, 41, 11)); // top (0)
            QueueBorderTiles.Add(new Rectangle(0, 12, 40, 25)); // sides (1)
            QueueBorderTiles.Add(new Rectangle(0, 38, 41, 11)); // bottom (2)
            QueueBorderTiles.Add(new Rectangle(0, 50, 22, 7)); // hold title (3)
            QueueBorderTiles.Add(new Rectangle(0, 58, 21, 7)); // next title (4)
            
            QueueBgTiles.Add(new Rectangle(0, 0, 34, 25)); // bg top (0)
            QueueBgTiles.Add(new Rectangle(0, 26, 34, 25)); // bg mid (1)
            QueueBgTiles.Add(new Rectangle(0, 52, 34, 25)); // bg bottom (2)
            QueueBgTiles.Add(new Rectangle(0, 78, 34, 25)); // bg static (3)
            

            QueueBorderTiles.Add(new Rectangle(40, 0, 41, 12)); // top hold
        }

        public Piece GetNextPiece() 
        {
            return _pieceQueue.Dequeue();
        }

        public virtual void Update() 
        {
            if (_pieceQueue.Count < 7)
                _pieceQueue.Enqueue(_generator.GetNextTetromino(_factory));
        }

        protected virtual void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset)
        {
            for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++) 
                {
                    if (piece.currentRotation[y, x] > 0) 
                    {
                        spriteBatch.Draw(
                            ImgBank.BlockTexture,
                            new Rectangle((int)(x * Tilesize + offset.X), (int)(y * Tilesize + offset.Y), Tilesize, Tilesize),
                            QueuePieceTiles[piece.currentRotation[y, x] - 1],
                            Color.White
                            );
                    }
                }
            }
        }

        private void DrawQueue(SpriteBatch spriteBatch) 
        {
            for (var i = 1; i <= 7; i++)
            {
                if ((i - 1) < QueueLength) 
                {
                    var buffer = _pieceQueue.ElementAt(i - 1).type switch
                    {
                        TetrominoType.O => 9,
                        TetrominoType.I => 1,
                        _ => 5
                    };
                    
                    var bufferY = _pieceQueue.ElementAt(i - 1).type switch
                    {
                        TetrominoType.I => 1,
                        _ => 5
                    };
                    
                    DrawPiece(spriteBatch, _pieceQueue.ElementAt(i - 1), new Vector2(Offset.X + buffer, ((i - 1) * Gridsize) + Offset.Y + bufferY));
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            for (var i = 0; i < QueueLength; i++)
            {
                spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 3, (i * Gridsize) + Offset.Y), QueueBorderTiles[1], Color.White);
                spriteBatch.Draw(BgTexture, new Vector2(Offset.X, (i * Gridsize) + Offset.Y), QueueBgTiles[3], Color.White);
            }
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 3, Offset.Y - 3), QueueBorderTiles[0], Color.White); // top
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 3, (QueueLength * Gridsize) + Offset.Y - 7), QueueBorderTiles[2], Color.White); // bottom
            spriteBatch.Draw(BorderTexture, new Vector2(Offset.X - 3 , Offset.Y - 11), QueueBorderTiles[4], Color.White);
            DrawQueue(spriteBatch);
        }
    }
}
