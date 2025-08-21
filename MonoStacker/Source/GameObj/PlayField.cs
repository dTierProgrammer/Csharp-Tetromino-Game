
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
using MonoStacker.Source.Interface;
using MonoStacker.Source.VisualEffects.Text;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.Data;

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
        PreGame,
        Playing,
        LineClearDelay,
        ArrivalDelay,
        RecieveDamage,
        TopOut,
        Finish
    }

    public enum BoardDisplaySetting 
    {
        BoardOnly,
        ShowMeter
    }

    public enum TopOutRule 
    {
        BlockOut,
        LockOut,
        PartialLockOut
    }

    public enum PreRrotationType 
    {
        IRS,
        Buffer,
        None
    }

    public enum PreHoldType 
    {
        IHS,
        Buffer,
        None
    }

    public enum DASChargeType 
    {
        Constant,
        Buffer
    }

    public enum InitialGarbageSystem // make into bools bro
    {
        Meter,
        Visualizer,
        Both,
        Disable
    }

    public enum ComboType 
    {
        Conventional,
        Arcade,
        Disable
    }

    public enum StreakType 
    {
        Flat,
        Incremental,
        Disable
    }

    public enum HardDropType // make into bools maybe
    {
        HardDrop,
        FirmDrop,
        Both,
        Disable
    }

    public enum RotationsAllowed 
    {
        CwOnly,
        CcwOnly,
        Both
    }

    enum HoriMovement 
    {
        Left = -1,
        Right = 1
    }


    public class PlayField
    {
        public readonly Vector2 offset;
        public readonly Grid grid;
        public int linesCleared { get; private set; }
        public int prevLinesCleared { get; private set; }
        public readonly BoardDisplaySetting displaySetting;
        private List<AnimatedEffect> _animatedEffects = new List<AnimatedEffect>();
        private ParticleLayer _particleLayer;
        private AnimatedEffectLayer _aeLayer = new();

        private KeyboardState _prevKbState;
        private readonly InputManager _inputManager;
        private InputDevice _inputDevice = InputDevice.Keyboard;

        private BoardState _currentBoardState = BoardState.PreGame;
        private BoardState _previousBoardState;
        private readonly IRotationSystem _rotationSystem;
        public Piece? activePiece { get; set; }
        private readonly Texture2D _border = GetContent.Load<Texture2D>("Image/Board/border1");
        private readonly Texture2D _border1 = GetContent.Load<Texture2D>("Image/Board/border0");
        private readonly Texture2D _borderMeterBg = GetContent.Load<Texture2D>("Image/Board/meter_bg");
        private readonly Texture2D _lockDelayMeter = GetContent.Load<Texture2D>("Image/Board/meter_hori");
        private bool _temporaryLandingSys = true;
        private (float timeLeftover, float max) _lineClearDelay;
        private (float timeLeftover, float max) _arrivalDelay;
        private (float timeLeftover, float max) _softLockDelay;
        private (int maxResets, int leftoverResets) _horiStepReset;
        private (int maxResets, int leftoverResets) _rotateReset;
        private bool _horiStepResetAllowed;
        private bool _vertStepResetAllowed;
        private float _prevYOff;
        private bool _rotateResetAllowed;
        private bool _isInDanger;
        private bool _spawnAreaObscured;

        private readonly float _maxDasTime;

        private float g_UnitX;
        private float _autoRepeatRate;
        private bool _softDrop;
        private readonly bool _softLock;

        public float gravity { get; set; }
        private float _softDropFactor;
        private float g_Unit;

        private readonly int _queueLength = 3;
        private readonly bool _holdEnabled;
        float time = 0;
        float rowTime = 0;
        private int _greyRow = Grid.ROWS - 1;
        private int _highestRow;

        private PieceManager _pieceManager;

        private bool _streakIsActive = false;
        private bool _bravo;

        public int _streak { get; private set; } = -1;
        public SpinType currentSpinType { get; private set; } = SpinType.None;
        private readonly SpinDenotation _parsedSpins = SpinDenotation.TSpinOnly;
        public int _combo { get; private set; } = -1;
        private readonly ComboType _comboType;
        private bool _singlesBreakCombo;

        private Texture2D _bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private readonly Dictionary<GameAction, float> _eventTimeStamps = [];
        private readonly List<GameAction> _currentInputEvents = [];
        private List<GameAction> _lastInputEvents = [];

        private float _lockDelayAmount;

        private bool _showDebug = false;

        public event Action PiecePlaced;
        public event Action ClearingLines;
        public event Action GenericSpinPing;
        public event Action StreakContinue;
        public event Action StreakBreak;
        public event Action ComboContinue;
        public event Action ComboBreak;
        public event Action Bravo;
        public event Action TopOut;
        public event Action Win;
        public event Action Loss;

        public PlayField(Vector2 position, PlayFieldData data, InputBinds binds)
        { // game feels ever so slightly less responsive now, will investigate
            _aeLayer = new();
            _particleLayer = new();
            offset = position - new Vector2(_bgTest.Width / 2, _bgTest.Height / 2);
            grid = new Grid(offset);
            _pieceManager = new
                (
                    data.factory,
                    data.spawnAreaOffset,
                    data.randomizer,
                    data.queueLength,
                    data.queueType,
                    data.holdEnabled
                );
            _rotationSystem = data.rotationSystem;
            _pieceManager.Initialize(this);
            _softLockDelay = (data.softLockDelay, data.softLockDelay);
            _vertStepResetAllowed = data.vertStepResetAllowed;
            _lineClearDelay = (data.lineClearDelay, data.lineClearDelay);
            _horiStepReset = (data.horiStepResets, data.horiStepResets);
            _horiStepResetAllowed = data.horiStepResetAllowed;
            _rotateReset = (data.rotateResets, data.rotateResets);
            _rotateResetAllowed = data.rotateResetAllowed;
            _arrivalDelay = (data.arrivalDelay, data.arrivalDelay);
            _softLock = data.softDropLocks;
            gravity = data.gravity;
            _softDropFactor = data.softDropFactor;
            _autoRepeatRate = data.autoshiftRepeatRate;
            _maxDasTime = data.autoshiftDelay;
            _comboType = data.comboType;
            _singlesBreakCombo = data.singlesBreakCombo;
            displaySetting = data.displaySetting;

            _inputManager = new InputManager(binds);
        }

        public void StartGame() 
        {
            GrabNextPiece();
        }

        public void EndGame() 
        {
            _highestRow = grid.GetHighestRow();
            _currentBoardState = BoardState.Finish;
            Debug.WriteLine("ellle");
        }

        private void MovePieceHorizontal(int movementAmt) // move the piece a given amount 
        {
            for (var i = 0; i < Math.Abs(movementAmt); i++)
            {
                if (!grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)(activePiece.offsetX + (1 * Math.Sign(movementAmt))))) break;
                activePiece.offsetX += (1 * Math.Sign(movementAmt));
                if (activePiece.offsetY == CalculateGhostPiece(activePiece) && _horiStepResetAllowed)
                    StepReset();
                SfxBank.stepHori.Play();
            }
        }
        private void MovePieceVertical(int movementAmt)
        {
            for (var i = 0; i < Math.Abs(movementAmt); i++)
            {
                if (!grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + (1 * Math.Sign(movementAmt))), (int)(activePiece.offsetX))) break;
                activePiece.offsetY += (1 * Math.Sign(movementAmt));

                if (_softDrop)
                    PlayfieldEffects.FlashPiece(activePiece, activePiece.color * .5f, 1f, offset, _aeLayer);

                if (!_vertStepResetAllowed) continue;
                _softLockDelay.timeLeftover = _softLockDelay.max;
            }
        }

        private void AutoShift(GameTime gameTime, HoriMovement movementDir) // move piece once every interval 
        {
            g_UnitX += _autoRepeatRate;
            while (g_UnitX > 1)
            {
                MovePieceHorizontal((int)movementDir);
                g_UnitX--;
            }
        }

        private bool CanDas(GameTime gameTime, float timeStamp) // determine when AutoShift() should kick in
        {
            return (float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= _maxDasTime;
        }

        private bool Rotate(RotationType rotation) // rotate the piece (always returns true lol)
        {
            currentSpinType = SpinType.None;
            if (_rotationSystem.Rotate(activePiece, grid, rotation))
            {
                currentSpinType = _parsedSpins switch
                {
                    SpinDenotation.TSpinOnly => activePiece.type is TetrominoType.T ? grid.CheckForSpin(activePiece) : SpinType.None,
                    _ => SpinType.None
                };

                if (currentSpinType is not SpinType.None) SfxBank.twist1m.Play();
                else SfxBank.rotate.Play();
            }
            if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece) && _rotateResetAllowed)
                RotateReset();
            activePiece.Update();
            return true;
        }

        private int CalculateGhostPiece(Piece piece) // get the ghost piece's position
        {
            var xOff = (int)piece.offsetX;
            var yOff = (int)piece.offsetY;
            while (grid.IsPlacementValid(piece, yOff + 1, xOff))
                yOff++;
            return yOff;
        }

        private int CalculateGhostPiece(Piece piece, int offsetX, int offsetY) // get the ghost piece's position from a specifiec offset
        {
            var xOff = offsetX;
            var yOff = offsetY;
            while (grid.IsPlacementValid(piece, yOff + 1, xOff))
                yOff++;
            return yOff;
        }

        private void IncrementCombo()
        {
            switch (_comboType)
            {
                case ComboType.Conventional:
                    _combo++;
                    ComboContinue?.Invoke();
                    break;
                case ComboType.Arcade:
                    if (grid.CheckForLines() > 1)
                    {
                        _combo++;
                        ComboContinue?.Invoke();
                    }
                    if (_singlesBreakCombo) { BreakCombo(); break; }

                    break;
            }
        }

        private void BreakCombo()
        {
            if (_combo > -1)
            {
                _combo = -1;
                ComboBreak?.Invoke();
            }
        }

        private void LockPiece() // lock piece onto the grid
        {
            grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            PiecePlaced?.Invoke();
            if (grid.CheckForLines() > 0)
            {
                IncrementCombo();
                _currentBoardState = BoardState.LineClearDelay;
                _lineClearDelay.timeLeftover = _lineClearDelay.max;
            }
            else
            {
                BreakCombo();
                _currentBoardState = BoardState.ArrivalDelay;
                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                if (currentSpinType != SpinType.None)
                    SfxBank.spinGeneric.Play();
            }

        }

        private bool RotateReset() // reset lock delay when the piece rotates while touching the stack
        {
            if (_rotateReset.leftoverResets <= 0) return false;
            _rotateReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private bool StepReset() // reset lock delay when the piece moves horizontally while touching the stack
        {
            if (_horiStepReset.leftoverResets <= 0) return false;
            _horiStepReset.leftoverResets--;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            return true;
        }

        private void GravitySoftDrop(GameTime gameTime) // anything related to gravity/softdropping the piece
        {

            var speed = _softDrop ? gravity * _softDropFactor : gravity;
            g_Unit += speed;
            while (g_Unit > 1)
            {
                if (grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + 1), (int)(activePiece.offsetX)))
                    MovePieceVertical(1);
                g_Unit--;
            }


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
            if ((int)activePiece.offsetY != CalculateGhostPiece(activePiece))
                activePiece.offsetY = CalculateGhostPiece(activePiece);
        }

        private void ClearFilledLines() // actually justs animates the line clear/plays the corresponding sounds/effects
        {

            if (grid.rowsToClear.Count == 4 || ((currentSpinType == SpinType.FullSpin || currentSpinType == SpinType.MiniSpin)))
            {
                _streakIsActive = true;
                _streak = _streakIsActive ? _streak += 1 : 0;
                if (_streak > 0) SfxBank.b2b.Play();
                StreakContinue?.Invoke();
            }
            else
            {
                if (_streakIsActive)
                {
                    _streakIsActive = false;
                    if (_streak > 0)
                        SfxBank.b2bBreak.Play();
                    _streak = -1;
                    StreakBreak?.Invoke();
                }
            }

            if (currentSpinType != SpinType.None) SfxBank.clearSpin[grid.rowsToClear.Count - 1].Play();
            else SfxBank.clear[grid.rowsToClear.Count - 1].Play();

            if (_lineClearDelay.timeLeftover != _lineClearDelay.max) return;
            PlayfieldEffects.LineClearFlash(Color.White, .5f, grid, offset);
            PlayfieldEffects.LineClearEffect(grid, offset);

            if (currentSpinType != SpinType.None)
                PlayfieldEffects.FlashPiece(activePiece, activePiece.color, .7f, new Vector2(.5f, .5f), offset);
        }

        private void DropLines() // actually clears the lines
        {
            grid.ClearLines();
            if (grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play();
            else Bravo?.Invoke();
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
                _currentBoardState = BoardState.TopOut;
                _highestRow = grid.GetHighestRow();
                TopOut?.Invoke();
            }
            else
            {
                
                _pieceManager.canHold = true;
                _softDrop = false;
                activePiece = _pieceManager.DealPiece();
                _currentBoardState = BoardState.Playing;
                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                currentSpinType = SpinType.None;
                _softLockDelay.timeLeftover = _softLockDelay.max;
                _horiStepReset.leftoverResets = _horiStepReset.maxResets;
                _rotateReset.leftoverResets = _rotateReset.maxResets;

                _spawnAreaObscured = ((WillPieceObscureSpawn(_pieceManager.pieceQueue.Peek())));
                _isInDanger = grid.GetNonEmptyRows() >= 18;
            }

        }

        private bool IsSpawnObscured() // check if the spawn area is obscured
        {
            for (var y = 0; y < _pieceManager.spawnArea.Length; y++)
            {
                for (var x = 0; x < _pieceManager.spawnArea[y].Length; x++)
                {
                    if (grid._matrix[y + _pieceManager.spawnAreaPosition.Y][x + _pieceManager.spawnAreaPosition.X] > 0)
                        return true;
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
                    if (_pieceManager.holdEnabled && HoldPiece() is not null) { SfxBank.holdBuffer.Play(); currentSpinType = SpinType.None; }
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
                    currentSpinType = SpinType.None;
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
            List<GameAction> heldActions = _inputManager.GetKeyInputSelection(new Keys[] { _inputManager._binds.k_MovePieceLeft, _inputManager._binds.k_MovePieceRight });
            if (heldActions.Contains(GameAction.MovePieceLeft))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceLeft))
                {
                    _eventTimeStamps.TryAdd(GameAction.MovePieceLeft, (float)gameTime.TotalGameTime.TotalSeconds);
                    MovePieceHorizontal(-1);
                }
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceRight);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft)))
                        AutoShift(gameTime, (HoriMovement)(-1));
                }
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceLeft);


            if (heldActions.Contains(GameAction.MovePieceRight))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceRight))
                {
                    _eventTimeStamps.TryAdd(GameAction.MovePieceRight, (float)gameTime.TotalGameTime.TotalSeconds);
                    MovePieceHorizontal(1);
                }
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceLeft);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight)))
                        AutoShift(gameTime, (HoriMovement)1);
                }
            }
            else
                _eventTimeStamps.Remove(GameAction.MovePieceRight);

        }

        public void Update(GameTime gameTime)
        {
            switch (_currentBoardState)
            {
                case BoardState.Playing:
                    if (_inputManager.bufferQueue.Count > 0) ProcessBuffer(); else ProcessButtonInput(gameTime);
                    break;
                case BoardState.LineClearDelay:
                    _inputManager.BufferKeyInput(gameTime);
                    if (_lineClearDelay.timeLeftover == _lineClearDelay.max)
                    {
                        ClearFilledLines();
                        ClearingLines?.Invoke();
                    }

                    _lineClearDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_lineClearDelay.timeLeftover <= 0)
                    {
                        DropLines();
                        _previousBoardState = BoardState.LineClearDelay;
                        _currentBoardState = BoardState.ArrivalDelay;
                        _arrivalDelay.timeLeftover = _arrivalDelay.max;

                    }
                    break;
                case BoardState.ArrivalDelay:
                    _inputManager.BufferKeyInput(gameTime);
                    if ((_arrivalDelay.timeLeftover == _arrivalDelay.max)) // marked
                    {
                        if (_previousBoardState is not BoardState.LineClearDelay)
                            PlayfieldEffects.FlashPiece(activePiece, activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f, offset);
                    }
                    _previousBoardState = _currentBoardState;
                    _arrivalDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_arrivalDelay.timeLeftover <= 0) 
                    {
                        GrabNextPiece();
                        //_currentBoardState = BoardState.Playing;
                    }
                    break;
                case BoardState.TopOut:
                    var interval = .05f;

                    rowTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
      
                    grid.SetDrawMode();
                    if (_greyRow >= _highestRow && rowTime >= interval)
                    {
                        rowTime = 0;
                        grid.ColorRow(_greyRow, 8);
                        _greyRow--;
                    }
                    break;
                case BoardState.Finish:
                    var intervalB = .15f;
                    rowTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    grid.SetDrawMode();
                    if (_greyRow >= _highestRow && rowTime >= intervalB)
                    {
                        rowTime = 0;
                        for (int i = 0; i < Grid.COLUMNS - 1; i++) 
                        {
                            if (grid._matrix[_greyRow][i] != 0)
                                AnimatedEffectManager.AddEffect(new LockFlash(ImgBank.BlockTexture, grid.imageTiles[grid._matrix[_greyRow][i] - 1],new Vector2(offset.X + (i * 8), offset.Y + (_greyRow * 8) - 160), Color.White, 1f, Vector2.Zero));
                        }
                           
                        grid.ColorRow(_greyRow, 0);
                        _greyRow--;
                    }
                    break;
            }
            if (_currentBoardState is BoardState.Playing || _currentBoardState is BoardState.LineClearDelay || _currentBoardState is BoardState.ArrivalDelay)
                ProcessDirectionalInput(gameTime); // true buffering doesn't work well with DAS, so you can just charge it at any time
            if (_currentBoardState is BoardState.Playing) 
            {
                GravitySoftDrop(gameTime);
                _lockDelayAmount = MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1);
            }
                
            _lastInputEvents = _inputManager.GetKeyInput();
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !_prevKbState.IsKeyDown(Keys.T))
                _showDebug = !_showDebug;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && !_prevKbState.IsKeyDown(Keys.Y))
                _temporaryLandingSys = !_temporaryLandingSys;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !_prevKbState.IsKeyDown(Keys.R))
                grid.ClearGrid();
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !_prevKbState.IsKeyDown(Keys.G)) 
            {
                if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece) && activePiece.offsetY > -1) 
                {
                    activePiece.offsetY--;
                }
                grid.AddGarbageLine(8);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && !_prevKbState.IsKeyDown(Keys.Tab)) 
                _combo++;

#endif
                _prevKbState = Keyboard.GetState();
            if(activePiece is not null)
                _prevYOff = activePiece.offsetY;
            _aeLayer.Update(gameTime);
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    if (piece.currentRotation[y, x] != 0)
                    {
                        if (_temporaryLandingSys)
                            spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (CalculateGhostPiece(activePiece) * 8) + (int)offset.Y - 160, 8, 8), grid.imageTiles[7], Color.White * .35f);
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), grid.imageTiles[piece.currentRotation[y, x] - 1], Color.Lerp(Color.DarkGray, Color.White, MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1)));
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
                            spriteBatch.Draw(grid.ghostBlocks, new Vector2((x * 8) + ((int)piece.offsetX * 8) + (int)offset.X, (y * 8) + (piece.offsetY * 8) + offset.Y - 160), grid.imageTiles[6], Color.White);
                    }
                }
            }
            if (!(_currentBoardState == BoardState.Playing && _showDebug)) return;
            
            for (var y = 0; y < piece.requiredCorners.GetLength(0); y++)
            {
                for (var x = 0; x < piece.requiredCorners.GetLength(1); x++)
                {
                    var cornerImg = piece.requiredCorners[y, x] switch
                    {
                        1 => grid.debugCM,
                        2 => grid.debugCO,
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
            grid.Draw(spriteBatch);
            if (displaySetting is BoardDisplaySetting.BoardOnly)
                spriteBatch.Draw(_border, new Vector2(offset.X - 5, offset.Y - 4), !_isInDanger ? Color.White : Color.Lerp(Color.White, Color.Red, (float)(Math.Sin(time * 2.0f) * .5f + .5f)));
            else
            {
                spriteBatch.Draw(_border1, new Vector2(offset.X - 11, offset.Y - 4), !_isInDanger ? Color.White : Color.Lerp(Color.White, Color.Red, (float)(Math.Sin(time * 2.0f) * .5f + .5f)));
                spriteBatch.Draw(_borderMeterBg, new Vector2(offset.X - 7, offset.Y), Color.White);
            }
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
                
            if (_currentBoardState == BoardState.Playing)
            {
                DrawPiece(spriteBatch, activePiece);
                # if DEBUG
                DrawPieceDB(spriteBatch, activePiece);
                #endif
            }
            _pieceManager.Draw(spriteBatch);
            
            if(_currentBoardState is not BoardState.TopOut && _spawnAreaObscured)
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
            
            spriteBatch.End();
        }
    }
}
