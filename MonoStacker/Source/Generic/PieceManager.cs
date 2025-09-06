using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic.Rotation;

namespace MonoStacker.Source.Generic
{
    public enum QueueType 
    {
        Top, // TGM styled queue
        Sides // Guideline styled queue
    }

    public class PieceManager
    {
        /*
        Intended to merge functionality of Hold/Next classes into one

        Initialize:
        - Piece factory
        - Piece randomizer
        - Queue length
        - Hold enabled?
        - Playfield (if necessary?)

        Function:
        - Deal pieces
        - Hold pieces if needed
         */
        // add functionality that adds any held inputs to the buffer (prevents dropped inputs if presses just miss buffer window)
        private PlayField _playfield;
        private QueueType _queueType;
        private Vector2 _sideQueueOffset;
        private Vector2 _sideHoldOffset;
        private Vector2 _topQueueOffset;
        private readonly int _queueLength;
        protected static readonly Texture2D BorderTexture = GetContent.Load<Texture2D>("Image/Board/queue0");
        protected static readonly Texture2D BorderTextrueAlt;
        protected static readonly Texture2D BgTexture = GetContent.Load<Texture2D>("Image/Board/queue_bg");
        protected static readonly Texture2D BgTextrueAlt;
        protected static readonly List<Rectangle> QueueBorderTiles = [];
        protected static readonly List<Rectangle> QueuePieceTiles = [];
        protected static readonly List<Rectangle> QueueBgTiles = [];
        protected const int Tilesize = 8;
        protected const int SmallTilesize = 6;
        protected const int Gridsize = 25;
        protected const int SmallGridsize = 17;
        public readonly Queue<Piece> pieceQueue;
        public readonly ITetrominoFactory _factory;
        public readonly int[][] spawnArea;
        public Point spawnAreaPosition;
        private readonly IRandomizer _generator;
        public readonly bool holdEnabled;
        public bool canHold;
        private Queue<Piece> _holdBox;

        private static void GetImageCuts()
        {
            QueuePieceTiles.Add(new Rectangle(0, 0, Tilesize, Tilesize)); // Cyan
            QueuePieceTiles.Add(new Rectangle(Tilesize, 0, Tilesize, Tilesize)); // Blue
            QueuePieceTiles.Add(new Rectangle(Tilesize * 2, 0, Tilesize, Tilesize)); // Orange
            QueuePieceTiles.Add(new Rectangle(Tilesize * 3, 0, Tilesize, Tilesize)); // Yellow
            QueuePieceTiles.Add(new Rectangle(Tilesize * 4, 0, Tilesize, Tilesize)); // Green
            QueuePieceTiles.Add(new Rectangle(Tilesize * 5, 0, Tilesize, Tilesize)); // Purple/Pink/Magenta
            QueuePieceTiles.Add(new Rectangle(Tilesize * 6, 0, Tilesize, Tilesize)); // Red
            QueuePieceTiles.Add(new Rectangle(0, 8, 8, 8));

            QueueBorderTiles.Add(new Rectangle(0, 0, 41, 11)); // top (0)
            QueueBorderTiles.Add(new Rectangle(0, 12, 40, 25)); // sides (1)
            QueueBorderTiles.Add(new Rectangle(0, 38, 41, 11)); // bottom (2)
            QueueBorderTiles.Add(new Rectangle(0, 50, 22, 7)); // hold title (3)
            QueueBorderTiles.Add(new Rectangle(0, 58, 21, 7)); // next title (4)

            QueueBorderTiles.Add(new Rectangle(0, 74, 44, 26)); // top queue main (5)
            QueueBorderTiles.Add(new Rectangle(0, 101, 24, 21)); // top queue hold (6)
            QueueBorderTiles.Add(new Rectangle(0, 123, 27, 26)); // top queue extra (7)

            QueueBgTiles.Add(new Rectangle(0, 0, 34, 25)); // bg top (0)
            QueueBgTiles.Add(new Rectangle(0, 26, 34, 25)); // bg mid (1)
            QueueBgTiles.Add(new Rectangle(0, 52, 34, 25)); // bg bottom (2)
            QueueBgTiles.Add(new Rectangle(0, 78, 34, 25)); // bg static (3)

            QueueBgTiles.Add(new Rectangle(0, 104, 36, 21)); // bg top queue main (4)
            QueueBgTiles.Add(new Rectangle(0, 126, 18, 13)); // bg top queue hold (5)
            QueueBgTiles.Add(new Rectangle(0, 140, 23, 13)); // bg top queue extra (6)


            QueueBorderTiles.Add(new Rectangle(40, 0, 41, 12)); // top hold
        }

        private void LoadQueue() 
        {
            for (var i = 0; i < _queueLength; i++)
                pieceQueue.Enqueue(_generator.GetNextTetromino(_factory));
        }

        public PieceManager(ITetrominoFactory factory, Point spawnAreaOffset, IRandomizer generator, int queueLength, QueueType queueDisplayType, bool holdEnabled) 
        {
            GetImageCuts();
            _factory = factory;
            spawnArea = _factory.SpawnArea();
            spawnAreaPosition = spawnAreaOffset;
            _generator = generator;
            _queueLength = queueLength;
            this.holdEnabled = holdEnabled;
            pieceQueue = new();
            _holdBox = new();
            _queueType = queueDisplayType;
            canHold = true;
            LoadQueue();
        }

        public void Initialize(PlayField playfield) 
        {
            _playfield = playfield;
        }

        public Piece DealPiece(RotationType? preRotateType, bool preHoldRequested) 
        {
            pieceQueue.Enqueue(_generator.GetNextTetromino(_factory));
            var piece = pieceQueue.Dequeue();

            if (preHoldRequested)
                piece = ChangePiece(piece);
            if (preRotateType.HasValue)
            {
                var buffer = piece.type switch
                {
                    TetrominoType.I => _factory.SpawnOffset_I(),
                    TetrominoType.O => _factory.SpawnOffset_O(),
                    _ => _factory.SpawnOffset_Jlstz()
                };
                switch (preRotateType)
                {
                    case RotationType.Clockwise:
                        if(_playfield.grid.IsDataPlacementValid(piece.rotations[piece.ProjectRotateCW()], spawnAreaPosition.Y + buffer.Y, spawnAreaPosition.X + buffer.X))
                            piece.RotateCW();
                        break;
                    case RotationType.CounterClockwise:
                        if (_playfield.grid.IsDataPlacementValid(piece.rotations[piece.ProjectRotateCCW()], spawnAreaPosition.Y + buffer.Y, spawnAreaPosition.X + buffer.X))
                            piece.RotateCCW();
                        break;
                }
            }
            
            SetPieceSpawn(piece);
            piece.Update();
            return piece;
        }

        public Piece ChangePiece(Piece piece) 
        {
            if(!holdEnabled) return null;
            if (!canHold) return null;
            canHold = false;
            ResetPiece(piece);
            if (_holdBox.Count == 0)
            {
                _holdBox.Enqueue(piece);
                return DealPiece(null, false);
            }
            var heldPiece = _holdBox.Dequeue();
            _holdBox.Enqueue(piece);
            return heldPiece;
        }

        private void SetPieceSpawn(Piece piece) // crash if piece is held right as one spawns at top of screen ?
        {
            piece.offsetX = spawnAreaPosition.X;
            piece.offsetY = spawnAreaPosition.Y;
            if (piece.type is TetrominoType.I) { piece.offsetX += _factory.SpawnOffset_I().X; piece.offsetY += _factory.SpawnOffset_I().Y; }
            else if (piece.type is TetrominoType.O) { piece.offsetX += _factory.SpawnOffset_O().X; piece.offsetY += _factory.SpawnOffset_O().Y; }
            else { piece.offsetX += _factory.SpawnOffset_Jlstz().X; piece.offsetY += _factory.SpawnOffset_Jlstz().Y; }
        }

        private void ResetPiece(Piece piece) 
        {
            SetPieceSpawn(piece);
            piece.ResetId();
            piece.Update();
        }

        private void DrawPiece(SpriteBatch spriteBatch, Piece piece, Vector2 offset, int tilesize, int? overrideId)
        {
            for (var y = 0; y < piece.thumbnail.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.thumbnail.GetLength(1); x++) 
                {
                    if (piece.thumbnail[y, x] > 0) 
                    {
                        spriteBatch.Draw(
                            ImgBank.BlockTexture,
                            new Rectangle((int)(x * tilesize + offset.X), (int)(y * tilesize + offset.Y), tilesize, tilesize),
                            overrideId is not null? QueuePieceTiles[overrideId.Value] :QueuePieceTiles[piece.thumbnail[y, x] - 1],
                            Color.White
                        );
                    }
                }
            }
        }

        private void DrawSideNextQueue(SpriteBatch spriteBatch, Vector2 queueOffset) 
        {
            for (var i = 0; i < _queueLength; i++) 
            {
                spriteBatch.Draw(BorderTexture, new Vector2(queueOffset.X - 3, (i * Gridsize) + queueOffset.Y), QueueBorderTiles[1], Color.White);
                spriteBatch.Draw(BgTexture, new Vector2(queueOffset.X, (i * Gridsize) + queueOffset.Y), QueueBgTiles[3], Color.White);
            }

            for (var i = 0; i < pieceQueue.Count; i++) 
            {
                var buffer = pieceQueue.ElementAt(i).type switch
                {
                    TetrominoType.O => 1,
                    TetrominoType.I => 1,
                    _ => 5
                };

                var bufferY = pieceQueue.ElementAt(i).type switch
                {
                    TetrominoType.I => 9,
                    _ => 5
                };

                DrawPiece(spriteBatch, pieceQueue.ElementAt(i), new Vector2(queueOffset.X + buffer, ((i) * Gridsize) + queueOffset.Y + bufferY), 8, null);
            }
               

            spriteBatch.Draw(BorderTexture, new Vector2(queueOffset.X - 3, queueOffset.Y - 3), QueueBorderTiles[0], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(queueOffset.X - 3, (_queueLength * Gridsize) + queueOffset.Y - 7), QueueBorderTiles[2], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(queueOffset.X - 3, queueOffset.Y - 11), QueueBorderTiles[4], Color.White);
        }

        private void DrawSideHoldQueue(SpriteBatch spriteBatch, Vector2 offset) // so much duplicated shi for no reason lol
        {
            for (var i = 0; i < 1; i++) 
            {
                spriteBatch.Draw(BgTexture, new Vector2(offset.X, (i * Gridsize) + offset.Y), QueueBgTiles[3], Color.White);
                spriteBatch.Draw(BorderTexture, new Vector2(offset.X - 3, (i * Gridsize) + offset.Y), QueueBorderTiles[1], Color.White);
            }

            if (_holdBox.Count != 0) 
            {
                var buffer = _holdBox.ElementAt(0).type switch
                {
                    TetrominoType.O => 1,
                    TetrominoType.I => 1,
                    _ => 5
                };

                var bufferY = _holdBox.ElementAt(0).type switch
                {
                    TetrominoType.I => 9,
                    _ => 5
                };

                DrawPiece(spriteBatch, _holdBox.ElementAt(0), new Vector2(offset.X + buffer, offset.Y + bufferY), 8, canHold ? null: 7);
            }

            spriteBatch.Draw(BorderTexture, new Vector2(offset.X - 4, offset.Y - 3), QueueBorderTiles[0], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X - 4, (1 * Gridsize) + offset.Y - 7), QueueBorderTiles[2], Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X - 4, offset.Y - 11), QueueBorderTiles[3], Color.White);
            
        }

        private void DrawTopNextQueue(SpriteBatch spriteBatch, Vector2 offset) 
        {
            
            spriteBatch.Draw(BgTexture, new Vector2(offset.X + 4, offset.Y + 4), QueueBgTiles[4], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X, offset.Y), QueueBorderTiles[5], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X + 4, offset.Y - 8), QueueBorderTiles[4], Color.White);
            DrawPiece(spriteBatch, pieceQueue.ElementAt(0), new Vector2(offset.X + 6, offset.Y + 6), 8, null);

            if (_queueLength > 1) 
            {
                spriteBatch.Draw(BorderTexture, new Vector2(offset.X + 40, offset.Y), QueueBorderTiles[7], Color.White);
                spriteBatch.Draw(BgTexture, new Vector2(offset.X + 44, offset.Y + 4), QueueBgTiles[6], Color.White);
                var bound = _queueLength <= 3 ? _queueLength - 1 : 2;
                for (var i = 0; i < bound; i++)
                    DrawPiece(spriteBatch, pieceQueue.ElementAt(i + 1), new Vector2((i * SmallGridsize) + offset.X + 41, offset.Y + 6), 4, null);
            }

            if (_queueLength > 3) 
            {
                for (var i = 0; i < _queueLength - 3; i++) 
                {
                    DrawPiece(spriteBatch, pieceQueue.ElementAt(i + 3), new Vector2(offset.X + 67, (i * 13) + offset.Y + 20), 4, null);
                }
            }
        }

        private void DrawTopHoldQueue(SpriteBatch spriteBatch, Vector2 offset) 
        {
            spriteBatch.Draw(BgTexture, new Vector2(offset.X + 5, offset.Y + 4), QueueBgTiles[5], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X, offset.Y), QueueBorderTiles[6], Color.White);
            spriteBatch.Draw(BorderTexture, new Vector2(offset.X, offset.Y - 8), QueueBorderTiles[3], Color.White);

            if (_holdBox.Count != 0) 
            {
                DrawPiece(spriteBatch, _holdBox.ElementAt(0), new Vector2(offset.X + 6, offset.Y + 6), 4, canHold ? null : 7);
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            switch (_queueType) 
            {
                case QueueType.Top:
                    DrawTopNextQueue(spriteBatch, new Vector2(_playfield.offset.X + 18, _playfield.offset.Y - 45));
                    if(holdEnabled)
                        DrawTopHoldQueue(spriteBatch, new Vector2(_playfield.offset.X - 5, _playfield.offset.Y - 45));
                    break;
                case QueueType.Sides:
                    DrawSideNextQueue(spriteBatch, new Vector2(_playfield.offset.X + 88, _playfield.offset.Y - 1));
                    DrawSideHoldQueue(spriteBatch, new Vector2(_playfield.displaySetting is BoardDisplaySetting.BoardOnly? _playfield.offset.X - 42 : _playfield.offset.X - 48, _playfield.offset.Y - 1));
                    break;
            }
        }
    }
}
