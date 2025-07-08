
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Generic.Rotation.RotationSystems;
using MonoStacker.Source.Interface.Input;

namespace MonoStacker.Source.GameObj
{
    

    public enum SpinDenotation 
    {
        None,
        TSpinOnly,
        TSpinSpecific,
        AllSpin
    }
    

    public enum BoardState 
    {
        GameStart,
        Neutral,
        LineClearWait,
        PieceEntryWait,
        GameEnd
    }

    public class PlayField
    {
        private readonly Vector2 _offset;
        private readonly Grid _grid;

        private KeyboardState _prevKbState;
        private readonly InputManager _inputManager;

        private BoardState _currentBoardState = BoardState.Neutral;
        //BoardState[] noDrawStates = { BoardState.LineClearWait, BoardState.PieceEntryWait, BoardState.GameEnd};
        private readonly IRotationSystem _rotationSystem;
        public Piece activePiece { get; set; }
        private Color _activePieceColor = Color.White;
        private readonly Texture2D _border = GetContent.Load<Texture2D>("Image/Board/border");
        private bool _showGhostPiece = true;
        private (float min, float max) _lcDelay;
        private (float min, float max) _arrivalDelay;
        private (float min, float max) _softLockDelay;
        private (int maxResets , int leftoverResets) _stepReset;
        private (int maxResets, int leftoverResets) _rotateReset;

        private readonly float _maxDasTime = .14f;
        
        private bool _softDrop;
        private float _dropSpeed;
        public NextPreview nextPreview { get; private set; }
        private readonly int _queueLength = 5;
        private HoldPreview _holdBox;
        private bool _streakIsActive = false;
        private int _streak;
        private SpinType _currentSpinType = SpinType.None;
        private readonly SpinDenotation _parsedSpins = SpinDenotation.TSpinOnly;

        private Texture2D _bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private readonly Dictionary<GameAction, float> _eventTimeStamps = [];
        private List<GameAction> _lastEvents = [];
        

#if DEBUG
        private bool _showDebug = true;
#else
        private bool _showDebug = false;
#endif

        public PlayField(Vector2 position)
        {
            _offset = position;
            _grid = new Grid(_offset);
            _softLockDelay = (.63f, .63f);
            _lcDelay = (.3f, .3f);
            _stepReset = (15, 15);
            _rotateReset = (6, 6);
            _arrivalDelay = (0, 0);
            nextPreview = new NextPreview(new Vector2((_border.Width) + _offset.X + 2, _offset.Y), _queueLength);
            activePiece = nextPreview.GetNextPiece();
            activePiece.offsetY = 20;
            activePiece.offsetX = 3;
            _inputManager = new InputManager();
            _rotationSystem = new SuperRotationSys();
        }
        
        public void Initialize() 
        {
            _holdBox = new HoldPreview(new Vector2(_offset.X - 42, _offset.Y), this);
        }

        private void MovePiece(float movementAmt)
        {
            if (!_grid.IsPlacementValid(activePiece, (int)activePiece.offsetY,
                    (int)(activePiece.offsetX + movementAmt)))
                return;
            activePiece.offsetX += movementAmt;
            SfxBank.stepHori.Play();
            if ((int)activePiece.offsetY == CalculateGhostPiece())
                StepReset();
        }

        private bool CanDas(GameTime gameTime, float timeStamp)
        {
            return (float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= _maxDasTime;
        }

        private bool Rotate(RotationType rotation)
        {
            _currentSpinType = SpinType.None;
            if (!_rotationSystem.Rotate(activePiece, _grid, rotation)) return false;
            _currentSpinType = _parsedSpins switch
            {
                SpinDenotation.TSpinOnly => activePiece is T? _grid.CheckForSpin(activePiece): SpinType.None,
                _ => SpinType.None
            };
            if((int)activePiece.offsetY == CalculateGhostPiece())
                RotateReset();
            activePiece.Update();
            return true;
        } 

        private int CalculateGhostPiece()
        {
            var xOff = (int)activePiece.offsetX;
            var yOff = (int)activePiece.offsetY;
            while (_grid.IsPlacementValid(activePiece, yOff + 1, xOff))
                yOff++;
            return yOff;
        } 

        private void LockPiece()
        {
            //activePieceColor = Color.White;
            //lockDelay = lockDelayMax;
            _softLockDelay.min = _softLockDelay.max;
            _stepReset.leftoverResets = _stepReset.maxResets;
            _rotateReset.leftoverResets = _rotateReset.maxResets;
            _currentSpinType = SpinType.None;
            _grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            if (_grid.CheckForLines() > 0) 
            {
                _currentBoardState = BoardState.LineClearWait;
                //_lineClearDelay = _lineClearDelayMax;
                _lcDelay.min = _lcDelay.max;
            }
            else
            {
                _currentBoardState = BoardState.PieceEntryWait;
                //_pieceEntryDelay = _pieceEntryDelayMax;
                _arrivalDelay.min = _arrivalDelay.max;
            }
            _holdBox.canHold = true;
        }

        private bool RotateReset()
        {
            if (_rotateReset.leftoverResets  <= 0) return false;
            _rotateReset.leftoverResets--;
            _softLockDelay.min = _softLockDelay.max;
            return true;
        }

        private bool StepReset()
        {
            if (_stepReset.leftoverResets  <= 0) return false;
            _stepReset.leftoverResets--;
            _softLockDelay.min = _softLockDelay.max;
            return true;
        }

        private void GravitySoftDrop(GameTime gameTime) 
        {
            _dropSpeed = _softDrop? .3f : .03f;

            if (activePiece.offsetY + _dropSpeed <= CalculateGhostPiece())
            {
                if (_grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + _dropSpeed), (int)activePiece.offsetX))
                    activePiece.offsetY += _dropSpeed;
            }
            else
                activePiece.offsetY = CalculateGhostPiece();

            if ((int)activePiece.offsetY == CalculateGhostPiece())
            {
                _softLockDelay.min -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                var lockDelayLeft = MathHelper.Clamp(_softLockDelay.min / _softLockDelay.max, 0, 1);
                _activePieceColor = Color.Lerp(new Color(90, 90, 90), Color.White, lockDelayLeft);

                if (_softLockDelay.min <= 0)
                {
                    LockPiece();
                    SfxBank.softLock.Play();
                }
            }
            else
                _activePieceColor = Color.White;
        }

        private void HardDrop()
        {
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece();
            SfxBank.hardDrop.Play();
        }

        private void ClearFilledLines()
        {
            //_lcDelay.min = _lcDelay.max;
            if ((int)_lcDelay.min != (int)_lcDelay.max) return;
            PlayfieldEffects.LineClearFlash(Color.White, .5f, _grid, _offset);
            PlayfieldEffects.LineClearEffect(_grid, _offset);
            switch (_grid.rowsToClear.Count()) 
            {
                case 1:
                    if (_currentSpinType != SpinType.None) SfxBank.clearSpin[0].Play();
                    else SfxBank.clear[0].Play(); break;
                case 2:
                    if (_currentSpinType != SpinType.None) SfxBank.clearSpin[1].Play();
                    else SfxBank.clear[1].Play(); break;
                case 3:
                    if (_currentSpinType != SpinType.None) SfxBank.clearSpin[2].Play();
                    else SfxBank.clear[2].Play(); break;
                default:
                    if (_currentSpinType != SpinType.None) SfxBank.clearSpin[3].Play();
                    else SfxBank.clear[3].Play(); break;
            }

            if (_currentSpinType != SpinType.None) // fake switch case
                PlayfieldEffects.FlashPiece(activePiece, activePiece.color, .7f, new Vector2(.5f, .5f), _offset);

            if (_grid.rowsToClear.Count == 4 || ((_currentSpinType == SpinType.FullSpin || _currentSpinType == SpinType.MiniSpin)))
            {
                if (!_streakIsActive)
                {
                    _streakIsActive = true; 
                    _streak = 0;
                }
                else
                {
                    _streak++;
                    SfxBank.b2b.Play();
                }
            }
            else
            {
                if (_streakIsActive)
                {
                    _streakIsActive = false;
                    _streak = 0;
                    SfxBank.b2bBreak.Play();
                }
            }
            _currentSpinType = SpinType.None;
        }

        private void DropLines()
        {
            _grid.ClearLines();
            if (_grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play();
            GrabNextPiece();
        }

        private void GrabNextPiece()
        {
            activePiece = nextPreview.GetNextPiece();
            _currentBoardState = BoardState.Neutral;
            _arrivalDelay.min = _arrivalDelay.max;
            _currentSpinType = SpinType.None;
        }

        private void ProcessBuffer()
        {
            var bufferedActions = _inputManager.GetBufferedActions();
            foreach (var item in bufferedActions)
            {
                var actionsStillHeld = _inputManager.GetKeyInput();
                if (actionsStillHeld.Contains(item.gameAction) && item.hasBeenExecuted == false)
                {
                    switch (item.gameAction)
                    {
                        case GameAction.Hold: 
                            if (_holdBox.SwapPiece()) SfxBank.holdBuffer.Play(); 
                            break;
                        case GameAction.RotateCcw: case GameAction.RotateCw:
                            Rotate(item.gameAction is GameAction.RotateCw ? 
                                RotationType.Clockwise : RotationType.CounterClockwise);
                            SfxBank.rotateBuffer.Play();
                            break;
                        case GameAction.MovePieceLeft: case GameAction.MovePieceRight: 
                            _eventTimeStamps.TryAdd(item.gameAction, item.timePressed);
                            break;
                    }
                }
                if (!item.hasBeenExecuted)
                    item.hasBeenExecuted = true;
            }
            if (_inputManager.bufferQueue.Count > 0)
                _inputManager.ClearBuffer(); 
        }

        private void ProcessInput(GameTime gameTime)
        {
            List<GameAction> heldActions = _inputManager.GetKeyInput();
            
            if (heldActions.Contains(GameAction.MovePieceLeft))
            {
                if (!_lastEvents.Contains(GameAction.MovePieceLeft))
                {
                    _eventTimeStamps.Add(GameAction.MovePieceLeft, (float)gameTime.TotalGameTime.TotalSeconds);
                    MovePiece(-1);
                }
                if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft))) MovePiece(-1);
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceLeft);


            if (heldActions.Contains(GameAction.MovePieceRight))
            {
                if (!_lastEvents.Contains(GameAction.MovePieceRight))
                { 
                    _eventTimeStamps.Add(GameAction.MovePieceRight, (float)gameTime.TotalGameTime.TotalSeconds); 
                    MovePiece(1);
                }
                if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight))) MovePiece(1);
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceRight);
                        
            _softDrop = heldActions.Contains(GameAction.SoftDrop);

            if (heldActions.Contains(GameAction.Hold) && !_lastEvents.Contains(GameAction.Hold)) 
            {
                if (_holdBox.SwapPiece())
                    SfxBank.hold.Play();
            }

            if (heldActions.Contains(GameAction.RotateCw) && !_lastEvents.Contains(GameAction.RotateCw)) 
            {
                Rotate(RotationType.Clockwise);
                SfxBank.rotate.Play();
            }

            if (heldActions.Contains(GameAction.RotateCcw) && !_lastEvents.Contains(GameAction.RotateCcw)) 
            {
                Rotate(RotationType.CounterClockwise);
                SfxBank.rotate.Play();
            }

            if (!heldActions.Contains(GameAction.HardDrop) ||
                _lastEvents.Contains(GameAction.HardDrop)) return;
            HardDrop();
        }

        public void Update(GameTime gameTime) 
        {
            nextPreview.Update();
            switch (_currentBoardState) 
            {
                case BoardState.Neutral:
                    if (_inputManager.bufferQueue.Count > 0)
                        ProcessBuffer();
                    else
                        ProcessInput(gameTime);
                    GravitySoftDrop(gameTime);
                    break;
                case BoardState.LineClearWait:
                    _inputManager.BufferKeyInput(gameTime);
                    if (_lcDelay.min == _lcDelay.max) // marked
                        ClearFilledLines(); //_lcDelay.min = _lcDelay.max;
                    
                    _lcDelay.min -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(_lcDelay.min <= 0)
                        DropLines();
                    break;
                case BoardState.PieceEntryWait:
                    _inputManager.BufferKeyInput(gameTime);
                    
                    if (_arrivalDelay.min == _arrivalDelay.max) // marked
                        PlayfieldEffects.FlashPiece(activePiece, activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f, _offset);
                    _arrivalDelay.min -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_arrivalDelay.min <= 0) 
                        GrabNextPiece();
                    break;
            }
            _lastEvents = _inputManager.GetKeyInput();

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !_prevKbState.IsKeyDown(Keys.T))
                _showDebug = !_showDebug;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && !_prevKbState.IsKeyDown(Keys.Y))
                        _showGhostPiece = !_showGhostPiece;
# endif
            _prevKbState = Keyboard.GetState();
        }

        private void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    var sourceRect = piece.currentRotation[y, x] switch
                    {
                        1 => _grid.imageTiles[0], 2 => _grid.imageTiles[1],
                        3 => _grid.imageTiles[2], 4 => _grid.imageTiles[3],
                        5 => _grid.imageTiles[4], 6 => _grid.imageTiles[5],
                        7 => _grid.imageTiles[6], _ => Rectangle.Empty
                    };

                    if (piece.currentRotation[y, x] != 0)
                    {
                        if (_showGhostPiece)
                            spriteBatch.Draw(_grid.ghostBlocks, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + (CalculateGhostPiece() * 8) + (int)_offset.Y - 160, 8, 8), sourceRect, Color.White * .5f);
                        spriteBatch.Draw(_grid.blocks, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8), sourceRect, _activePieceColor);
                    }
                }
            }

            if (!(_currentBoardState == BoardState.Neutral && _showDebug)) return;
            
            for (var y = 0; y < piece.requiredCorners.GetLength(0); y++)
            {
                for (var x = 0; x < piece.requiredCorners.GetLength(1); x++)
                {
                    var cornerImg = piece.requiredCorners[y, x] switch
                    {
                        1 => _grid.debugCM,
                        2 => _grid.debugCO,
                        3 => GetContent.Load<Texture2D>("Image/Block/cenPt"),
                        _ => null
                    };
                        
                    if (piece.requiredCorners[y, x] > 0) 
                        spriteBatch.Draw(cornerImg, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8), Color.White);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(bgTest, _offset, Color.White);
            _grid.Draw(spriteBatch);
            spriteBatch.Draw(_border, new Vector2(_offset.X - 4, _offset.Y - 4), Color.White);
            if(_currentBoardState == BoardState.Neutral)
                DrawPiece(spriteBatch, activePiece);
            nextPreview.Draw(spriteBatch);
            _holdBox.Draw(spriteBatch);
            spriteBatch.End();
            
        }
    }
}
