
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
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
        DamageReeling,
        GameEnd
    }

    public class PlayField
    {
        public readonly Vector2 offset;
        public readonly ITetrominoFactory _pieceData;
        private readonly IRandomizer _pieceGenerator;
        private readonly Grid _grid;
        private List<AnimatedEffect> _animatedEffects = new List<AnimatedEffect>();

        private KeyboardState _prevKbState;
        private readonly InputManager _inputManager;

        private BoardState _currentBoardState = BoardState.Neutral;
        private BoardState _previousBoardState;
        //BoardState[] noDrawStates = { BoardState.LineClearWait, BoardState.PieceEntryWait, BoardState.GameEnd};
        private readonly IRotationSystem _rotationSystem;
        public Piece activePiece { get; set; }
        private Color _activePieceColor = Color.White;
        private readonly Texture2D _border = GetContent.Load<Texture2D>("Image/Board/border1");
        private readonly Texture2D _lockDelayMeter = GetContent.Load<Texture2D>("Image/Board/meter_hori");
        private bool _showGhostPiece = true;
        private (float timeLeftover, float max) _lineClearDelay;
        private (float timeLeftover, float max) _arrivalDelay;
        private (float timeLeftover, float max) _softLockDelay;
        private (int maxResets , int leftoverResets) _horiStepReset;
        private (int maxResets, int leftoverResets) _rotateReset;
        private bool _horiStepResetAllowed;
        private bool _vertStepResetAllowed;
        private float _prevYOff;
        private bool _rotateResetAllowed;

        private readonly float _maxDasTime = .14f;
        
        private bool _softDrop;
        private readonly bool _softLock;
        private float _dropSpeed;
        private readonly int _queueLength = 3;
        private readonly bool _holdEnabled;


        private PieceManager _pieceManager;


        private bool _streakIsActive = false;
        private int _streak;
        private SpinType _currentSpinType = SpinType.None;
        private readonly SpinDenotation _parsedSpins = SpinDenotation.TSpinOnly;

        private Texture2D _bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");
        private Texture2D _topQueueTest = GetContent.Load<Texture2D>("Image/Board/topqueue_1");
        
        private readonly Dictionary<GameAction, float> _eventTimeStamps = [];
        private List<GameAction> _currentInputEvents = []; //= _inputManager.GetKeyInput();
        private List<GameAction> _lastInputEvents = [];
        private float _apXCenter;
        private float _apYCenter;

        private float _lockDelayAmount;

#if DEBUG
        private bool _showDebug = false;
#else
        private bool _showDebug = false;
#endif

        public PlayField(Vector2 position)
        {
            offset = position;
            _grid = new Grid(offset);
            _pieceData = new SrsFactory();
            _pieceGenerator = new SevenBagRandomizer();
            _softLockDelay = (.5f, .5f);
            _vertStepResetAllowed = true;
            _lineClearDelay = (.5f, .5f);
            _horiStepReset = (15, 15);
            _horiStepResetAllowed = true;
            _rotateReset = (6, 6);
            _rotateResetAllowed = true;
            _arrivalDelay = (.5f, .5f);
            _holdEnabled = true;
            _softLock = false;
            
            _rotationSystem = new SuperRotationSys();
            _inputManager = new InputManager();
        }
        
        public void Initialize() 
        {
            _pieceManager = new(this, _pieceData, _pieceGenerator, _queueLength, _holdEnabled);
            activePiece = _pieceManager.DealPiece();
            _apXCenter = activePiece.currentRotation.GetLength(1) / 2;
            _apYCenter = activePiece.currentRotation.GetLength(0) / 2;
        }

        private void MovePiece(float movementAmt)
        {
            if (!_grid.IsPlacementValid(activePiece, (int)activePiece.offsetY,
                    (int)(activePiece.offsetX + movementAmt)))
                return;
            activePiece.offsetX += movementAmt;
            SfxBank.stepHori.Play();
            if ((int)activePiece.offsetY == CalculateGhostPiece() && _horiStepResetAllowed)
                StepReset();
        }

        private bool CanDas(GameTime gameTime, float timeStamp)
        {
            return (float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= _maxDasTime;
        }

        private bool Rotate(RotationType rotation)
        {
            _currentSpinType = SpinType.None;
            if (_rotationSystem.Rotate(activePiece, _grid, rotation))
            {
                _currentSpinType = _parsedSpins switch
                {
                    SpinDenotation.TSpinOnly => activePiece.type is TetrominoType.T? _grid.CheckForSpin(activePiece): SpinType.None,
                    _ => SpinType.None
                };

                if (_currentSpinType is not SpinType.None) SfxBank.twist1m.Play();
                else SfxBank.rotate.Play();
            }
            if((int)activePiece.offsetY == CalculateGhostPiece() && _rotateResetAllowed)
                RotateReset();
            activePiece.Update();
            _apXCenter = (activePiece.currentRotation.GetLength(1) * 8) / 2;
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
            _activePieceColor = Color.White;
            _grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            if (_grid.CheckForLines() > 0) 
            {
                _currentBoardState = BoardState.LineClearWait;
                _lineClearDelay.timeLeftover = _lineClearDelay.max;
            }
            else
            {
                _currentBoardState = BoardState.PieceEntryWait;
                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                if (_currentSpinType != SpinType.None)
                    SfxBank.spinGeneric.Play();
            }
        }

        private bool RotateReset()
        {
            if (_rotateReset.leftoverResets  <= 0) return false;
            _rotateReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private bool StepReset()
        {
            if (_horiStepReset.leftoverResets  <= 0) return false;
            _horiStepReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private void GravitySoftDrop(GameTime gameTime) 
        {
            _dropSpeed = _softDrop? 1 : 0;

            if (activePiece.offsetY + _dropSpeed <= CalculateGhostPiece())
            {
                if (_grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + _dropSpeed), (int)activePiece.offsetX)) 
                {
                    activePiece.offsetY += _dropSpeed;
                    if (!_vertStepResetAllowed) return;
                    if((int)activePiece.offsetY != (int)_prevYOff)
                    _softLockDelay.timeLeftover = _softLockDelay.max;
                }
            }
            else
                activePiece.offsetY = CalculateGhostPiece();

            if ((int)activePiece.offsetY == CalculateGhostPiece())
            {
                if (_softLock && _softDrop) 
                {
                    LockPiece();
                    SfxBank.softLock.Play();
                }
                    
                _softLockDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
               

                if (_softLockDelay.timeLeftover <= 0)
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
            //_animatedEffects.Add(new DropEffect(_offset, activePiece, CalculateGhostPiece(), 3f, Color.Red));
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece();
            SfxBank.hardDrop.Play();
        }

        private void FirmDrop()
        {
            if((int)activePiece.offsetY != CalculateGhostPiece())
                activePiece.offsetY = CalculateGhostPiece();
        }

        private void ClearFilledLines()
        {
            if (_lineClearDelay.timeLeftover != _lineClearDelay.max) return;
            PlayfieldEffects.LineClearFlash(Color.White, .5f, _grid, offset);
            PlayfieldEffects.LineClearEffect(_grid, offset);
            if (_currentSpinType != SpinType.None) SfxBank.clearSpin[_grid.rowsToClear.Count - 1].Play();
            else SfxBank.clear[_grid.rowsToClear.Count - 1].Play();

            if (_currentSpinType != SpinType.None)
                PlayfieldEffects.FlashPiece(activePiece, activePiece.color, .7f, new Vector2(.5f, .5f), offset);

            if (_grid.rowsToClear.Count == 4 || ((_currentSpinType == SpinType.FullSpin || _currentSpinType == SpinType.MiniSpin)))
            {
                _streakIsActive = true;
                _streak = _streakIsActive ? _streak += 1 : 0;
                if(_streak > 1) SfxBank.b2b.Play();
            }
            else
            {
                if (_streakIsActive)
                {
                    _streakIsActive = false;
                    _streak = 0;
                    if(_streak > 1)
                        SfxBank.b2bBreak.Play();
                }
            }
        }

        private void DropLines()
        {
            _grid.ClearLines();
            if (_grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play();
            //GrabNextPiece();
            //_currentSpinType = SpinType.None;
        }

        private Piece HoldPiece() 
        {
            var piece = _pieceManager.ChangePiece(activePiece);
            if (piece is null) return null;
            else activePiece = piece;
            return piece;
        }

        private void GrabNextPiece()
        {
            _pieceManager.canHold = true;
            activePiece = _pieceManager.DealPiece();
            _currentBoardState = BoardState.Neutral;
            _arrivalDelay.timeLeftover = _arrivalDelay.max;
            _currentSpinType = SpinType.None;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            _horiStepReset.leftoverResets = _horiStepReset.maxResets;
            _rotateReset.leftoverResets = _rotateReset.maxResets;
        }

        private void ProcessBuffer()
        {
            var bufferedActions = _inputManager.GetBufferedActions();
            var actionsStillHeld = _inputManager.GetKeyInput();
            foreach (var item in bufferedActions) // super scuffed method of buffer priority
            { // ensure that no matter what, hold (if buffered) is always executed first
                if (item.gameAction is GameAction.Hold && actionsStillHeld.Contains(item.gameAction) &&
                    item.hasBeenExecuted is false)
                    if (_pieceManager.holdEnabled && HoldPiece() is not null) { SfxBank.holdBuffer.Play(); _currentSpinType = SpinType.None; }
            }
            bufferedActions.RemoveAll(item => item.gameAction is GameAction.Hold);
            
            foreach (var item in bufferedActions)
            {
                if (actionsStillHeld.Contains(item.gameAction) && item.hasBeenExecuted is false)
                {
                    switch (item.gameAction)
                    {
                        case GameAction.RotateCcw: 
                            Rotate(RotationType.CounterClockwise);
                            SfxBank.rotateBuffer.Play();
                            break;
                        case GameAction.RotateCw:
                            Rotate(RotationType.Clockwise);
                            SfxBank.rotateBuffer.Play();
                            break;
                    }
                }
                item.hasBeenExecuted = !item.hasBeenExecuted;
            }
            _inputManager.ClearBuffer(); 
        }

        private void ProcessButtonInput(GameTime gameTime)
        {
            List<GameAction> heldActions = _inputManager.GetKeyInput();
            _softDrop = heldActions.Contains(GameAction.SoftDrop);

            if (heldActions.Contains(GameAction.Hold) && !_lastInputEvents.Contains(GameAction.Hold)) 
            {
                if (_pieceManager.holdEnabled && HoldPiece() is not null)
                {
                    SfxBank.hold.Play();
                    _currentSpinType = SpinType.None;
                    _softLockDelay.timeLeftover = _softLockDelay.max;
                }
            }

            if (heldActions.Contains(GameAction.RotateCw) && !_lastInputEvents.Contains(GameAction.RotateCw)) 
                Rotate(RotationType.Clockwise);

            if (heldActions.Contains(GameAction.RotateCcw) && !_lastInputEvents.Contains(GameAction.RotateCcw)) 
                Rotate(RotationType.CounterClockwise);

            if (!heldActions.Contains(GameAction.HardDrop) ||
                _lastInputEvents.Contains(GameAction.HardDrop)) return;
            HardDrop();
        }

        private void ProcessDirectionalInput(GameTime gameTime)
        {
            List<GameAction> heldActions = _inputManager.GetKeyInput();
            if (heldActions.Contains(GameAction.MovePieceLeft))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceLeft))
                {
                    _eventTimeStamps.TryAdd(GameAction.MovePieceLeft, (float)gameTime.TotalGameTime.TotalSeconds);
                    MovePiece(-1);
                }
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceRight);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft)))
                        MovePiece(-1);
                }
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceLeft);
            
            if (heldActions.Contains(GameAction.MovePieceRight))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceRight))
                { 
                    _eventTimeStamps.TryAdd(GameAction.MovePieceRight, (float)gameTime.TotalGameTime.TotalSeconds); 
                    MovePiece(1);
                }
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceLeft);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight)))
                        MovePiece(1);
                }
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceRight);
        }

        private void UpdateEffects(float deltaTime)
        {
            foreach (var effect in _animatedEffects)
                effect.Update(deltaTime);
            _animatedEffects.RemoveAll(effect => effect.TimeDisplayed <= 0);
        }

        private void DrawEffects(SpriteBatch spriteBatch)
        {
            foreach(var effect in _animatedEffects)
                effect.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime) 
        {
            _apXCenter = (activePiece.currentRotation.GetLength(1) * 8) / 2;
            Console.WriteLine(activePiece.GetPixelCenterOfRotation());
            if(_currentBoardState is not BoardState.Neutral)
                _inputManager.BufferKeyInput(gameTime);
            switch (_currentBoardState) 
            {
                case BoardState.Neutral:
                    if (_inputManager.bufferQueue.Count > 0) ProcessBuffer(); else ProcessButtonInput(gameTime);
                    break;
                case BoardState.LineClearWait:
                    if (_lineClearDelay.timeLeftover == _lineClearDelay.max) // marked
                        ClearFilledLines(); //_lcDelay.min = _lcDelay.max;
                    
                    _lineClearDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_lineClearDelay.timeLeftover <= 0) 
                    {
                        DropLines();
                        _previousBoardState = BoardState.LineClearWait;
                        _currentBoardState = BoardState.PieceEntryWait;
                        _arrivalDelay.timeLeftover = _arrivalDelay.max;
                       
                    }
                   
                    break;
                case BoardState.PieceEntryWait:
                    if ((_arrivalDelay.timeLeftover == _arrivalDelay.max)) // marked
                    {
                        if(_previousBoardState is not BoardState.LineClearWait)
                            PlayfieldEffects.FlashPiece(activePiece, activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f, offset);
                    }
                    _previousBoardState = _currentBoardState;
                    _arrivalDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_arrivalDelay.timeLeftover <= 0) 
                        GrabNextPiece();
                    break;
            }
            ProcessDirectionalInput(gameTime); // true buffering doesn't work well with DAS, so you can just charge it at any time
            if (_currentBoardState is BoardState.Neutral) 
            {
                GravitySoftDrop(gameTime);
                _lockDelayAmount = MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1);
            }
                
            _lastInputEvents = _inputManager.GetKeyInput();

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !_prevKbState.IsKeyDown(Keys.T))
                _showDebug = !_showDebug;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && !_prevKbState.IsKeyDown(Keys.Y))
                        _showGhostPiece = !_showGhostPiece;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !_prevKbState.IsKeyDown(Keys.R))
                _grid.ClearGrid();
#endif
                _prevKbState = Keyboard.GetState();
                
            UpdateEffects((float)gameTime.ElapsedGameTime.TotalSeconds); // end of loop ?
            _prevYOff = activePiece.offsetY;
        }

        private void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    if (piece.currentRotation[y, x] != 0)
                    {
                        if (_showGhostPiece)
                            spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (CalculateGhostPiece() * 8) + (int)offset.Y - 160, 8, 8), _grid.imageTiles[7], Color.White * .35f);
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), _grid.imageTiles[piece.currentRotation[y, x] - 1], Color.Lerp(Color.DarkGray, Color.White, MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1)));
                    }
                }
            }
        }
        private void DrawPieceDB(SpriteBatch spriteBatch, Piece piece) 
        {
            for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    if (piece.currentRotation[y, x] != 0)
                    {
                        if(_showDebug)
                            spriteBatch.Draw(_grid.ghostBlocks, new Vector2((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (piece.offsetY * 8) + offset.Y - 160), _grid.imageTiles[6], Color.White);
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
                    
                }
            }
            
            spriteBatch.Draw
            (
                GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"),
                new Rectangle
                (
                    (int)activePiece.GetPixelCenterOfRotation().X + ((int)activePiece.offsetX * 8) + (int)offset.X,
                    (int)activePiece.GetPixelCenterOfRotation().Y + ((int)activePiece.offsetY * 8) + (int)offset.Y - 160,
                    1,
                    1
                ),
                Color.Magenta
            );
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(_bgTest, _offset, Color.White);
            spriteBatch.Draw(ImgBank.GridBg, offset, Color.White);
            _grid.Draw(spriteBatch);
            spriteBatch.Draw(_border, new Vector2(offset.X - 5, offset.Y - 4), Color.White);
            spriteBatch.Draw(_lockDelayMeter, new Vector2(offset.X - 5, offset.Y + 165), Color.White);
            //spriteBatch.Draw(_topQueueTest, new Vector2(_offset.X - 5, _offset.Y - 43), Color.White);
            spriteBatch.Draw
            (
                GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"),
                new Rectangle((int)offset.X - 2, (int)offset.Y + 168, (int)MathHelper.Lerp(0, 84, _lockDelayAmount), 1),
                Color.Lerp(new Color(255, 0, 0), new Color(0, 255, 0), _lockDelayAmount)
            );
            DrawEffects(spriteBatch);
            if (_currentBoardState == BoardState.Neutral)
            {
                DrawPiece(spriteBatch, activePiece);
                # if DEBUG
                DrawPieceDB(spriteBatch, activePiece);
                #endif
            }
            _pieceManager.Draw(spriteBatch);
            spriteBatch.End();
            
        }
    }
}
