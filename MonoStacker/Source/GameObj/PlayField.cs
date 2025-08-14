
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
using System.Diagnostics;
using RasterFontLibrary.Source;

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

    public enum PreRrotationType 
    {
        IRS,
        Buffer
    }

    public enum ControllerType 
    {
        Keyboard,
        Gamepad
    }

    public class PlayField
    {
        public readonly Vector2 offset;
        private readonly Grid _grid;
        private List<AnimatedEffect> _animatedEffects = new List<AnimatedEffect>();
        private AnimatedEffectLayer _aeLayer = new();

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
        private bool _isInDanger;
        private bool _spawnAreaObscured;

        private readonly float _maxDasTime = .15f;

        private (float timer, float interval) _autoRepeatRate;
        private bool _softDrop;
        private readonly bool _softLock;
        private float _dropSpeed;
        private float _softDropFactor;
        private readonly int _queueLength = 3;
        private readonly bool _holdEnabled;
        float time = 0;
        float colorTime = 0;
        float colorTime2 = 0;
        float rowTime = 0;
        int buffer = Grid.ROWS - 1;

        private PieceManager _pieceManager;

        private bool _streakIsActive = false;
        private int _streak = -1;
        private Color streakColor;
        private SpinType _currentSpinType = SpinType.None;
        private readonly SpinDenotation _parsedSpins = SpinDenotation.TSpinOnly;
        private int _combo = -1;
        private Color comboColor = Color.White;
        private List<(string text, float timeDisplayed, Color color)> _actionText;

        private Texture2D _bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");
        
        private readonly Dictionary<GameAction, float> _eventTimeStamps = [];
        private readonly List<GameAction> _currentInputEvents = []; //= _inputManager.GetKeyInput();
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
            _aeLayer = new();
            offset = position;
            _grid = new Grid(offset);
            _pieceManager = new
                (
                    new SrsFactory(),
                    new Point(3, 18),
                    new SevenBagRandomizer(),
                    5,
                    QueueType.Sides,
                    true
                );
            _pieceManager.Initialize(this);
            GrabNextPiece();
            _rotationSystem = new SuperRotationSys();
            _softLockDelay = (.5f, .5f);
            _vertStepResetAllowed = false;
            _lineClearDelay = (0, 0);
            _horiStepReset = (15, 15);
            _horiStepResetAllowed = true;
            _rotateReset = (6, 6);
            _rotateResetAllowed = true;
            _arrivalDelay = (0, 0);
            _softLock = false;
            _dropSpeed = .03f;
            _softDropFactor = 40;
            _autoRepeatRate = (0, 0);
            _maxDasTime = .15f;
            _actionText = new();

            _inputManager = new InputManager();
        }

        private void MovePiece(float movementAmt) // move the piece a given amount 
        {
            if (!_grid.IsPlacementValid(activePiece, (int)activePiece.offsetY,
                    (int)(activePiece.offsetX + movementAmt)))
                return;
            activePiece.offsetX += movementAmt;
            SfxBank.stepHori.Play();
            if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece) && _horiStepResetAllowed)
                StepReset();
        }

        private void AutoRepeatMovement(GameTime gameTime, int movementAmt) // move piece once every interval 
        {
            if (_autoRepeatRate.interval >= 0)
            {
                _autoRepeatRate.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_autoRepeatRate.timer >= _autoRepeatRate.interval)
                {
                    MovePiece(movementAmt);
                    _autoRepeatRate.timer = 0;
                }
            }
            else 
            {
                while (true) 
                {
                    if (!_grid.IsPlacementValid(activePiece, (int)activePiece.offsetY,
                    (int)(activePiece.offsetX + movementAmt))) break;
                    MovePiece(movementAmt);
                }
            }
        }

        private bool CanDas(GameTime gameTime, float timeStamp) // determine when Delayed Auto Shift should kick in
        {
            return (float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= _maxDasTime;
        }

        private bool Rotate(RotationType rotation) // rotate the piece
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
            if((int)activePiece.offsetY == CalculateGhostPiece(activePiece) && _rotateResetAllowed)
                RotateReset();
            activePiece.Update();
            _apXCenter = (activePiece.currentRotation.GetLength(1) * 8) / 2;
            return true;
        } 

        private int CalculateGhostPiece(Piece piece) // get the ghost piece's position
        {
            var xOff = (int)piece.offsetX;
            var yOff = (int)piece.offsetY;
            while (_grid.IsPlacementValid(piece, yOff + 1, xOff))
                yOff++;
            return yOff;
        }

        private int CalculateGhostPiece(Piece piece, int offsetX, int offsetY) // get the ghost piece's position from a specifiec offset
        {
            var xOff = offsetX;
            var yOff = offsetY;
            while (_grid.IsPlacementValid(piece, yOff + 1, xOff))
                yOff++;
            return yOff;
        }

        private void LockPiece() // lock piece onto the grid
        {
            _activePieceColor = Color.White;
            _grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            if (_grid.CheckForLines() > 0) 
            {
                _combo++;
                comboColor = Color.White;
                Debug.WriteLine(_combo);
                _currentBoardState = BoardState.LineClearWait;
                _lineClearDelay.timeLeftover = _lineClearDelay.max;
            }
            else
            {
                if (_combo > -1) 
                {
                    _combo = -1;
                    Debug.WriteLine("combo break, reset to -1");
                }
                
                _currentBoardState = BoardState.PieceEntryWait;
                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                if (_currentSpinType != SpinType.None)
                    SfxBank.spinGeneric.Play();
            }
        }

        private bool RotateReset() // reset lock delay when the piece rotates while touching the stack
        {
            if (_rotateReset.leftoverResets  <= 0) return false;
            _rotateReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private bool StepReset() // reset lock delay when the piece moves horizontally while touching the stack
        {
            if (_horiStepReset.leftoverResets  <= 0) return false;
            _horiStepReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private void GravitySoftDrop(GameTime gameTime) // anything related to gravity/softdropping the piece
        {
            
            var speed = _softDrop ? _dropSpeed * _softDropFactor : _dropSpeed;
            if (activePiece.offsetY + speed <= CalculateGhostPiece(activePiece))
            {
                if (_grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + speed), (int)activePiece.offsetX)) 
                {
                    activePiece.offsetY += speed;
                    if (!_vertStepResetAllowed) return;
                    if((int)activePiece.offsetY != (int)_prevYOff)
                    _softLockDelay.timeLeftover = _softLockDelay.max;
                }
            }
            else
                activePiece.offsetY = CalculateGhostPiece(activePiece);

            if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece))
            {
                if ((int)activePiece.offsetY != (int)_prevYOff)
                    SfxBank.stackHit.Play();

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

            if (_softDrop) // issues at certain soft drop factors
            {
                if ((int)activePiece.offsetY != (int)_prevYOff)
                    PlayfieldEffects.FlashPiece(activePiece, activePiece.color * .5f, 1f, offset, _aeLayer);
            }
        }

        private void HardDrop() // place the piecec at the ghost piece's offset, lock it onto the grid
        {
            _aeLayer.AddEffect(new DropEffect(offset, 1f, activePiece, ((int)CalculateGhostPiece(activePiece) - (int)activePiece.offsetY), activePiece.color));
            activePiece.offsetY = CalculateGhostPiece(activePiece);
            PlayfieldEffects.DropSparkle(activePiece, offset);
            LockPiece();
            SfxBank.hardDrop.Play();
        }

        private void FirmDrop() // place the piece at the ghost piece's offset (but don't lock it)
        {
            if((int)activePiece.offsetY != CalculateGhostPiece(activePiece))
                activePiece.offsetY = CalculateGhostPiece(activePiece);
        }

        private void ClearFilledLines() // actually justs animates the line clear/plays the corresponding sounds/effects
        {
            if (_grid.rowsToClear.Count == 4 || ((_currentSpinType == SpinType.FullSpin || _currentSpinType == SpinType.MiniSpin)))
            {
                _streakIsActive = true;
                _streak = _streakIsActive ? _streak += 1 : 0;
                if(_streak > 0) SfxBank.b2b.Play();
                streakColor = Color.White;
            }
            else
            {
                if (_streakIsActive)
                {
                    _streakIsActive = false;
                    if(_streak > 0)
                        SfxBank.b2bBreak.Play();
                    _streak = -1;
                }
            }
            

            _actionText.Add(_grid.rowsToClear.Count switch 
            { // rewrite this lol
                1 => (_currentSpinType is SpinType.None? "Single": _currentSpinType is SpinType.FullSpin? "T-Spin Single": "Mini T-Spin Single", 2, _currentSpinType is SpinType.None ? Color.LightBlue: Color.Magenta),
                2 => (_currentSpinType is SpinType.None ? "Double" : "T-Spin Double", 2, _currentSpinType is SpinType.None ? Color.Yellow : Color.Magenta),
                3 => (_currentSpinType is SpinType.None ? "Triple" : "T-Spin Triple", 2, _currentSpinType is SpinType.None ? Color.Orange : Color.Magenta),
                4 => ("Quadruple", 2, Color.LightBlue)
            });

            if (_currentSpinType != SpinType.None) SfxBank.clearSpin[_grid.rowsToClear.Count - 1].Play();
            else SfxBank.clear[_grid.rowsToClear.Count - 1].Play();

            if (_lineClearDelay.timeLeftover != _lineClearDelay.max) return;
            PlayfieldEffects.LineClearFlash(Color.White, .5f, _grid, offset);
            PlayfieldEffects.LineClearEffect(_grid, offset);

            if (_currentSpinType != SpinType.None)
                PlayfieldEffects.FlashPiece(activePiece, activePiece.color, .7f, new Vector2(.5f, .5f), offset);
        }

        private void DropLines() // actually clears the lines
        {
            _grid.ClearLines();
            if (_grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play();
            
        }

        private Piece HoldPiece() // calls the piecemanager's hold method, return a piece if hold is succesful, else return null 
        {
            var piece = _pieceManager.ChangePiece(activePiece);
            if (piece is null) return null;
            else activePiece = piece;
            return piece;
        }

        private void GrabNextPiece() // sets the active piece to the first piece in the next queue
        {
            if (IsSpawnObscured())
            {
                _isInDanger = false;
                _currentBoardState = BoardState.GameEnd;
            }
            else 
            {
                _currentBoardState = BoardState.Neutral;
                _pieceManager.canHold = true;
                _softDrop = false;
                activePiece = _pieceManager.DealPiece();

                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                _currentSpinType = SpinType.None;
                _softLockDelay.timeLeftover = _softLockDelay.max;
                _horiStepReset.leftoverResets = _horiStepReset.maxResets;
                _rotateReset.leftoverResets = _rotateReset.maxResets;

                _spawnAreaObscured = ((WillPieceObscureSpawn(_pieceManager.pieceQueue.Peek())));
                _isInDanger = _grid.GetNonEmptyRows() >= 18;
            }
            
        }

        private bool IsSpawnObscured() // check if the spawn area is obscured
        {
            for (var y = 0; y < _pieceManager.spawnArea.Length; y++)
            {
                for (var x = 0; x < _pieceManager.spawnArea[y].Length; x++)
                {
                    if (_grid._matrix[y + _pieceManager.spawnAreaPosition.Y][x + _pieceManager.spawnAreaPosition.X] > 0) 
                    {
                        return true;
                    }
                }
            }
            return false; 
        }

        private bool WillPieceObscureSpawn(Piece piece) // check if the spawn area will be obscured in the future 
        {
            var buffer = _pieceManager.pieceQueue.Peek().type switch
            {
                TetrominoType.I => _pieceManager._factory.SpawnOffset_I(),
                TetrominoType.O => _pieceManager._factory.SpawnOffset_O(),
                _ => _pieceManager._factory.SpawnOffset_Jlstz()
            };

            if (CalculateGhostPiece(piece, _pieceManager.spawnAreaPosition.X + buffer.X, _pieceManager.spawnAreaPosition.Y + buffer.Y) <= _pieceManager.spawnAreaPosition.Y + 1)
                return true;
            else
                return false;
        }

        private void ProcessBuffer() // if inputs are buffered, executed associated actions in 1 frame
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

        private void ProcessButtonInput(GameTime gameTime) // if button inputs are read, executed associated actions
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

        private void ProcessDirectionalInput(GameTime gameTime) // if directional inputs are read, execute associated actions
        {
            List<GameAction> heldActions = _inputManager.GetKeyInputSelection(new Keys[] { Keys.Left, Keys.Right});
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
                    {
                        _autoRepeatRate.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (_autoRepeatRate.timer >= _autoRepeatRate.interval)
                        {
                            AutoRepeatMovement(gameTime, -1);

                        }
                    }
                }
            }
            else 
            {
                _eventTimeStamps.Remove(GameAction.MovePieceLeft);
                //_autoRepeatRate.timer = 0;
            }
                
            
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
                    {
                        _autoRepeatRate.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (_autoRepeatRate.timer >= _autoRepeatRate.interval)
                        {
                            AutoRepeatMovement(gameTime, 1);
                        }
                    }
                        
                }
            }
            else 
            {
                _eventTimeStamps.Remove(GameAction.MovePieceRight);
                //_autoRepeatRate.timer = 0;
            }
                
        }

        private void UpdateActionText(GameTime gameTime)
        {
            if (streakColor != Color.Cyan)
            {
                colorTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                streakColor = Color.Lerp(streakColor, Color.Cyan, colorTime / 2);
            }
            else
                colorTime = 0;

            if (comboColor != Color.Orange)
            {
                colorTime2 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                comboColor = Color.Lerp(comboColor, Color.Orange, time / 2);
            }
            else
                colorTime2 = 0;
            if (_actionText.Count > 0) 
            {
                for (int x = 0; x < _actionText.Count; x++)
                {
                    var action = _actionText[x];
                    action.timeDisplayed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    action.color = action.color * .994f;
                    _actionText[x] = action;
                }
            }
            _actionText.RemoveAll(item => item.timeDisplayed <= 0);
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
                    _inputManager.BufferKeyInput(gameTime);
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
                    _inputManager.BufferKeyInput(gameTime);
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
                case BoardState.GameEnd:
                    var interval = .1f;
                    rowTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _grid.SetDrawMode();
                    if (buffer > 0 && rowTime >= interval) 
                    {
                        rowTime = 0;
                        _grid.ColorRow(buffer, 8);
                        buffer--;
                    }
                        
                    break;
            }
            if(_currentBoardState is not BoardState.GameEnd)
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
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !_prevKbState.IsKeyDown(Keys.G)) 
            {
                if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece)) 
                {
                    activePiece.offsetY--;
                }
                _grid.AddGarbageLine(8);
            }
                
#endif
                _prevKbState = Keyboard.GetState();


            _aeLayer.Update(gameTime);
            _prevYOff = activePiece.offsetY;

            

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateActionText(gameTime);
            Debug.WriteLine(_actionText.Count);
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
                            spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (CalculateGhostPiece(activePiece) * 8) + (int)offset.Y - 160, 8, 8), _grid.imageTiles[7], Color.White * .35f);
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), _grid.imageTiles[piece.currentRotation[y, x] - 1], Color.Lerp(Color.DarkGray, Color.White, MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1)));
                    }
                }
            }
        }

        private void DrawPieceDanger(SpriteBatch spriteBatch, Piece piece) // super lazy reduntant methods yayaya
        {
            var buffer = _pieceManager.pieceQueue.Peek().type switch
            {
                TetrominoType.I => _pieceManager._factory.SpawnOffset_I(),
                TetrominoType.O => _pieceManager._factory.SpawnOffset_O(),
                _ => _pieceManager._factory.SpawnOffset_Jlstz()
            };

            for (var y = 0; y < piece.currentRotation.GetLength(0); y++)
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    if (piece.currentRotation[y, x] != 0)
                    {
                        spriteBatch.Draw
                            (GetContent.Load<Texture2D>("Image/Block/spawnPt"), 
                            new Rectangle((x * 8) + ((int)_pieceManager.spawnAreaPosition.X * 8) + (buffer.X * 8) + (int)offset.X, (y * 8) + ((int)_pieceManager.spawnAreaPosition.Y * 8) + (buffer.Y * 8) + (int)offset.Y - 160, 8, 8), 
                            Color.White);
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
                    if (piece.requiredCorners[y, x] != 0)
                    {
                        
                            spriteBatch.Draw(cornerImg, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), Color.White);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(_bgTest, _offset, Color.White);
            spriteBatch.Draw(ImgBank.GridBg, offset, Color.White);
            _aeLayer.Draw(spriteBatch);
            _grid.Draw(spriteBatch);
            spriteBatch.Draw(_border, new Vector2(offset.X - 5, offset.Y - 4), !_isInDanger? Color.White: Color.Lerp(Color.White, Color.Red, (float)(Math.Sin(time * 2.0f) * .5f + .5f)));
            spriteBatch.Draw(_lockDelayMeter, new Vector2(offset.X - 5, offset.Y + 165), Color.White);
            spriteBatch.Draw
            (
                GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"),
                new Rectangle((int)offset.X - 2, (int)offset.Y + 168, (int)MathHelper.Lerp(0, 84, _lockDelayAmount), 1),
                Color.Lerp(new Color(255, 0, 0), new Color(0, 255, 0), _lockDelayAmount)
            );
            
            if (_isInDanger) 
            {
                float alpha = (float)(Math.Sin(time * 2.0f) * .5f + .5f);
                spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Board/bg_top_gradient"), offset, Color.Red * alpha * .45f);
            }
                
            if (_currentBoardState == BoardState.Neutral)
            {
                
                DrawPiece(spriteBatch, activePiece);
                # if DEBUG
                DrawPieceDB(spriteBatch, activePiece);
                #endif
            }
            _pieceManager.Draw(spriteBatch);

            //if ((WillPieceObscureSpawn(_pieceManager.pieceQueue.Peek())))
            
            if(_currentBoardState is not BoardState.GameEnd && _spawnAreaObscured)
                DrawPieceDanger(spriteBatch, _pieceManager.pieceQueue.Peek());


# if DEBUG
            for (var y = 0; y < _pieceManager.spawnArea.Length; y++) 
            {
                for (var x = 0; x < _pieceManager.spawnArea[y].Length; x++) 
                {
                    if(_showDebug) spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Block/spawnPt"), new Vector2(offset.X + (x * 8) + (_pieceManager.spawnAreaPosition.X * 8), offset.Y + (y * 8) + (_pieceManager.spawnAreaPosition.Y * 8) - 160), Color.White * .63f);
                }
            }
#endif

            if (_currentBoardState is not BoardState.GameEnd && _combo > 0) 
                Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(offset.X - 6, offset.Y + 41), $"Combo *{_combo}", comboColor , OriginSetting.BottomRight);
            if (_currentBoardState is not BoardState.GameEnd && _streak > 0)
                Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(offset.X - 6, offset.Y + 49), $"B2B *{_streak}", streakColor, OriginSetting.BottomRight);
            if (_actionText.Count > 0)
            {
                for (var i = 0; i < _actionText.Count; i++)
                {
                    Font.DefaultSmallOutlineGradient.RenderString(
                        spriteBatch,
                        new Vector2(offset.X - 6, offset.Y + 57 + (i * 8)),
                        _actionText[i].text,
                        _actionText[i].color,
                        OriginSetting.BottomRight
                    );
                }
            }

            spriteBatch.End();
       
        }
    }
}
