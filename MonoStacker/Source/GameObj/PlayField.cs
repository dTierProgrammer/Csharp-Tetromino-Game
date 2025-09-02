
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Generic.Rotation.RotationSystems;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.VisualEffects.ParticleSys;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using MonoStacker.Source.VisualEffects.Text;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

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
        EventPause,
        LineClearDelay,
        ArrivalDelay,
        RecieveDamage,
        TopOut,
        Finish
    }

    enum NextAction
    {
        Continue,
        None
    }

    public enum FastDropType 
    {
        Disable,
        HardDrop,
        FirmDrop
    }

    public enum BoardDisplaySetting 
    {
        BoardOnly,
        ShowMeter
    }

    public enum BufferType
    {
        Tap,
        Hold,
        None
    }

    public enum TopOutRule 
    {
        BlockOut,
        LockOut,
        SpawnCover,
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

    public enum LineClearDelayType 
    {
        FlatDelay,
        IndividualDelay
    }

    public enum SoftDropType 
    {
        Factor,
        Set
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
        private NextAction _nextAction = NextAction.Continue;

        private KeyboardState _prevKbState;
        private readonly InputManager _inputManager;
        private (float timeLeftover, float max) _dasCut;
        private (float timeLeftover, float max) _hardDropCut;
        private InputDevice _inputDevice = InputDevice.Keyboard;
        private BufferType _bufferType;
        private RotationType? preRotationType;
        private bool preHoldRequested;
        private FastDropType _fastDropType;

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
        private float[] _individualLcDelays;
        private LineClearDelayType _lineClearDelayType;
        private (float timeLeftover, float max) _arrivalDelay;
        private (float timeLeftover, float max) _softLockDelay;
        private (int maxResets, int leftoverResets) _horiStepReset;
        private (int maxResets, int leftoverResets) _rotateReset;
        private bool _horiStepResetAllowed;
        private bool _vertStepResetAllowed;
        private int _prevYOff;
        private int _prevXOff;
        private bool _rotateResetAllowed;
        private bool _isInDanger;
        private bool _spawnAreaObscured;

        private readonly float _maxDasTime;

        private float g_UnitX;
        private float _autoRepeatRate;
        private bool _softDrop;
        private readonly bool _softLock;

        public float gravity { get; set; }
        private SoftDropType _softDropType;
        private float _softDropFactor;
        private float _softDropAmount;
        private float g_Unit;

        private readonly int _queueLength = 3;
        private readonly bool _holdEnabled;
        float time = 0;
        float rowTime = 0;
        private int _greyRow = Grid.ROWS - 1  ;
        private int _highestRow;

        private PieceManager _pieceManager;

        private bool _streakIsActive = false;
        private bool _bravo;

        public int _streak { get; private set; } = -1;
        public SpinType currentSpinType { get; private set; } = SpinType.None;
        public readonly SpinDenotation parsedSpins;
        public int _combo { get; private set; } = -1;
        private readonly ComboType _comboType;
        private bool _singlesBreakCombo;

        private Texture2D _bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private readonly Dictionary<GameAction, float> _eventTimeStamps = [];
        private List<GameAction> _currentInputEvents = [];
        private List<GameAction> _lastInputEvents = [];

        private float _lockDelayAmount;
        private TopOutRule _topOutRule;

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
        public event Action GameEnd;
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
            parsedSpins = data.parsedSpins;
            _rotationSystem = data.rotationSystem;
            _pieceManager.Initialize(this);
            _softLockDelay = (data.softLockDelay, data.softLockDelay);
            _vertStepResetAllowed = data.vertStepResetAllowed;
            _lineClearDelay = (data.lineClearDelay, data.lineClearDelay);
            _individualLcDelays = data.individualLcDelays;
            _lineClearDelayType = data.lineClearDelayType;
            _horiStepReset = (data.horiStepResets, data.horiStepResets);
            _horiStepResetAllowed = data.horiStepResetAllowed;
            _rotateReset = (data.rotateResets, data.rotateResets);
            _rotateResetAllowed = data.rotateResetAllowed;
            _arrivalDelay = (data.arrivalDelay, data.arrivalDelay);
            _softLock = data.softDropLocks;
            gravity = data.gravity;
            _softDropType = data.softDropType;
            _softDropFactor = data.softDropFactor;
            _softDropAmount = data.softDropAmount;
            _autoRepeatRate = data.autoshiftRepeatRate;
            _maxDasTime = data.autoshiftDelay;
            _comboType = data.comboType;
            _singlesBreakCombo = data.singlesBreakCombo;
            displaySetting = data.displaySetting;
            _dasCut = (0, data.dasCut);
            _hardDropCut = (0, data.hardDropCut);
            _fastDropType = data.fastDropType;
            _topOutRule = data.topOutRule;
            

            _inputManager = new InputManager(binds);
            _bufferType = data.bufferType;
        }

        public void Start() 
        {
            GrabNextPiece();
        }

        public void End() 
        {
            _highestRow = grid.GetHighestRow();
            _currentBoardState = BoardState.Finish;
        }

        public void PauseForEvent()
        {
            activePiece = null;
            _nextAction = NextAction.None;
        }

        private void GetInput(InputDevice device, PlayerIndex player)
        {
            switch (device)
            {
                case InputDevice.Gamepad:
                    _currentInputEvents = _inputManager.GetButtonInput(player);
                    break;
                case InputDevice.Keyboard:
                    _currentInputEvents = _inputManager.GetKeyInput();
                    break;
            }
        }

        private void MovePieceHorizontal(int movementAmt) // move the piece a given amount 
        {
            if (activePiece is null) return;
            for (var i = 0; i < Math.Abs(movementAmt); i++)
            {
                if (!grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)(activePiece.offsetX + (1 * Math.Sign(movementAmt))))) break;
                activePiece.offsetX += (1 * Math.Sign(movementAmt));
                if (activePiece.offsetY == CalculateGhostPiece(activePiece)) 
                {
                    if(_horiStepResetAllowed)
                        StepReset();
                    HitEffect(Color.Yellow);
                }
                SfxBank.stepHori.Play();
            }
        }

        private void MovePieceVertical(int movementAmt)
        {
            if (activePiece is null) return;
            for (var i = 0; i < Math.Abs(movementAmt); i++)
            {
                if (!grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + (1 * Math.Sign(movementAmt))), (int)(activePiece.offsetX))) 
                {
                    break;
                } 
                activePiece.offsetY += (1 * Math.Sign(movementAmt));

                if (_softDrop)
                    PlayfieldEffects.FlashPiece(activePiece, activePiece.color * .5f, 1f, offset, _aeLayer);

                if (!_vertStepResetAllowed) continue;
                _softLockDelay.timeLeftover = _softLockDelay.max;
            }
        }

        private void AutoShift(GameTime gameTime, HoriMovement movementDir) // move piece once every interval 
        {
            if (activePiece is null) return;
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
            if (activePiece is null) return false;
            currentSpinType = SpinType.None;
            if (_rotationSystem.Rotate(activePiece, grid, rotation))
            {
                activePiece.Update();
                currentSpinType = grid.CheckForSpin(activePiece);
                if (activePiece.offsetY != CalculateGhostPiece(activePiece))
                    currentSpinType = SpinType.None;
                else
                {
                    if (activePiece.offsetY - _prevYOff == 2)
                        currentSpinType = SpinType.FullSpin;
                }
                Debug.WriteLine(parsedSpins);
                if (parsedSpins == SpinDenotation.None)
                { 
                    currentSpinType = SpinType.None; Debug.WriteLine("piece of fucking shit program FUCK YOU"); 
                }
                else if (parsedSpins == SpinDenotation.TSpinOnly)
                    if (activePiece.type is not TetrominoType.T) currentSpinType = SpinType.None;
            }
                if (currentSpinType is not SpinType.None) SfxBank.twist1m.Play();
                else SfxBank.rotate.Play();
            if ((int)activePiece.offsetY == CalculateGhostPiece(activePiece) && _rotateResetAllowed)
                RotateReset();
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
            if (activePiece is null) return;
            if (activePiece.offsetY > -1)
                grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            else
            { Kill(); return; }    
                PiecePlaced?.Invoke();
            if (grid.CheckForLines() > 0)
            {
                IncrementCombo();
                ClearFilledLines();
                ClearingLines?.Invoke();
                _currentBoardState = BoardState.LineClearDelay;
                _lineClearDelay.timeLeftover = _lineClearDelay.max;
                if (_lineClearDelayType == LineClearDelayType.IndividualDelay) 
                    _lineClearDelay.timeLeftover = _individualLcDelays[grid.CheckForLines() - 1];
            }
            else
            {
                if (currentSpinType != SpinType.None)
                { SfxBank.spinGeneric.Play(); GenericSpinPing?.Invoke(); }
                BreakCombo();
                _currentBoardState = BoardState.ArrivalDelay;
                _arrivalDelay.timeLeftover = _arrivalDelay.max;
                PlayfieldEffects.FlashPiece(activePiece, activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f, offset);
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

        private void HitEffect(Color color) // move to playfieldEffects class
        {
            StaticEmissionSources sources = new([]);
            for (var y = 0; y < activePiece.currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < activePiece.currentRotation.GetLength(1); x++) 
                {
                    if (activePiece.currentRotation[y, x] > 0) 
                    {
                        if (activePiece.offsetY + y + 1 >= Grid.ROWS || grid._matrix[activePiece.offsetY + y + 1][activePiece.offsetX + x] > 0) 
                        {
                            AnimatedEffectManager.AddEffect(new LockFlash(GetContent.Load<Texture2D>("Image/Effect/HitEffect"), new Vector2(offset.X + ((activePiece.offsetX + x) * 8), offset.Y + ((activePiece.offsetY + y + 1) * 8) - 160 - 1), color, .3f));
                            sources.Members.Add(new GroupPartData()
                            {
                                Position = new Vector2(offset.X + ((activePiece.offsetX + x) * 8 + 4), offset.Y + ((activePiece.offsetY + y + 1) * 8) - 160 - 1),
                                Data = new EmitterData
                                {
                                    emissionInterval = 1f,
                                    density = 3,
                                    angleVarianceMax = 90,
                                    particleActiveTime = (.01f, .3f),
                                    speed = (50, 100),
                                    particleData = new ParticleData()
                                    {
                                        texture = GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"),
                                        colorTimeLine = (color, Color.White),
                                        scaleTimeLine = new(1.5f, 1),
                                        opacityTimeLine = new(1, 1),
                                        frictionFactor = new Vector2(0, 0)
                                    }
                                }
                            });
                        }
                    }
                }
            }

            GroupEmitterObj effect = new(sources, EmissionType.Burst);
            ParticleManager.AddEmitter(effect);
        }

        private void GravitySoftDrop(GameTime gameTime) // anything related to gravity/softdropping the piece
        {
            if (activePiece is null) return;
           
            var speed = _softDrop ? gravity * _softDropFactor : gravity;

            if (!grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + 1), (int)(activePiece.offsetX)))
            {
                g_Unit = 0;

                if (activePiece.offsetY != _prevYOff)
                {
                    Debug.WriteLine("ssskasj");
                    SfxBank.stackHit.Play();
                    if (speed > .5f)
                        HitEffect(new Color(214, 255, 255));

                }

                if (_softLock && _softDrop)
                {
                    Debug.WriteLine("hotaro");
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
            _prevYOff = activePiece.offsetY;

            if (_softDropType is SoftDropType.Set)
                speed = _softDrop ? _softDropAmount : gravity;
            g_Unit += speed;
            while (g_Unit > 1)
            {
                if (grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + 1), (int)(activePiece.offsetX)))
                    MovePieceVertical(1);
                g_Unit--;
            }
            
        }

        private void HardDrop() // place the piecec at the ghost piece's offset, lock it onto the grid
        {
            if (_currentBoardState is not BoardState.Playing) return;
            _aeLayer.AddEffect(new DropEffect(offset, 1f, activePiece, ((int)CalculateGhostPiece(activePiece) - (int)activePiece.offsetY), activePiece.color));
            activePiece.offsetY = CalculateGhostPiece(activePiece);
            PlayfieldEffects.DropSparkle(activePiece, offset);
            HitEffect(Color.Orange);
            LockPiece();
            SfxBank.hardDrop.Play();
        }

        private void FirmDrop() // place the piece at the ghost piece's offset (but don't lock it)
        {
            if (activePiece is not null && activePiece.offsetY != CalculateGhostPiece(activePiece)) 
            {
                _aeLayer.AddEffect(new DropEffect(offset, 1f, activePiece, ((int)CalculateGhostPiece(activePiece) - (int)activePiece.offsetY), activePiece.color));
                activePiece.offsetY = CalculateGhostPiece(activePiece);
                HitEffect(new Color(214, 255, 255));
            }
                
        }

        private void ClearFilledLines() // actually animates the line clear/plays the corresponding sounds/effects
        { // refactor to use event action delegate
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

            if(grid.GetNonEmptyRows() - grid.rowsToClear.Count == 0)
                Bravo?.Invoke();

            if (currentSpinType != SpinType.None) SfxBank.clearSpin[grid.rowsToClear.Count - 1].Play();
            else SfxBank.clear[grid.rowsToClear.Count - 1].Play();
            
            PlayfieldEffects.LineClearFlash(Color.White, .5f, grid, offset);
            PlayfieldEffects.LineClearEffect(grid, offset);

            if (currentSpinType != SpinType.None && activePiece is not null)
                PlayfieldEffects.FlashPiece(activePiece, activePiece.color, .7f, new Vector2(.5f, .5f), offset);
        }

        private void DropLines() // actually clears the lines
        {
            grid.ClearLines();
            if (grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play();
            
        }

        private Piece HoldPiece() // calls the piecemanager's hold method, return a piece if hold is succesful, else return null 
        {
            if (activePiece is null) return null;
            var piece = _pieceManager.ChangePiece(activePiece);
            if (piece is null) return null;
            else activePiece = piece;
            return piece;
        }

        private void Kill() 
        {
            _isInDanger = false;
            _currentBoardState = BoardState.TopOut;
            _highestRow = grid.GetHighestRow();
            TopOut?.Invoke();
        }

        private void GrabNextPiece() // sets the active piece to the first piece in the next queue
        {
            if (grid.overflowRows.Count > 0)
            { Kill(); return; }
            switch (_topOutRule) 
            {
                case TopOutRule.BlockOut:
                    if (CheckBlockOut())
                    { Kill(); return; }
                    break;
                case TopOutRule.LockOut:
                    if (grid.GetHighestRow() < _pieceManager.spawnAreaPosition.Y || CheckBlockOut())
                    { Kill(); return; }
                    break;
                case TopOutRule.SpawnCover:
                    if (IsSpawnObscured())
                    { Kill(); return; }
                    break;
            }
            

            _dasCut.timeLeftover = _dasCut.max;
            _hardDropCut.timeLeftover = _hardDropCut.max;
            _pieceManager.canHold = true;
            _softDrop = false;
            activePiece = _pieceManager.DealPiece(preRotationType, preHoldRequested);
            if (preRotationType.HasValue)
                SfxBank.rotateBuffer.Play();
            if (preHoldRequested)
                SfxBank.holdBuffer.Play();
            preRotationType = null;
            preHoldRequested = false;
            _currentBoardState = BoardState.Playing;
            _arrivalDelay.timeLeftover = _arrivalDelay.max;
            currentSpinType = SpinType.None;
            _softLockDelay.timeLeftover = _softLockDelay.max;
            _horiStepReset.leftoverResets = _horiStepReset.maxResets;
            _rotateReset.leftoverResets = _rotateReset.maxResets;

            _spawnAreaObscured = ((WillPieceObscureSpawn(_pieceManager.pieceQueue.Peek())));
            _isInDanger = grid.GetNonEmptyRows() >= 18;

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

        private bool CheckBlockOut() 
        {
            var buffer = _pieceManager.pieceQueue.Peek().type switch
            {
                TetrominoType.I => _pieceManager._factory.SpawnOffset_I(),
                TetrominoType.O => _pieceManager._factory.SpawnOffset_O(),
                _ => _pieceManager._factory.SpawnOffset_Jlstz()
            };
            if (!grid.IsPlacementValid(_pieceManager.pieceQueue.Peek(), _pieceManager.spawnAreaPosition.Y + buffer.Y, _pieceManager.spawnAreaPosition.X + buffer.X))
            {
                grid.LockPiece(_pieceManager.pieceQueue.Peek(), _pieceManager.spawnAreaPosition.Y + buffer.Y, _pieceManager.spawnAreaPosition.X + buffer.X);
                _pieceManager.DealPiece(null, false);
                return true; 
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

        private void ProcessBuffer() // if inputs are buffered, executed associated actions in 1 frame (deprecated in favor of IHS/IRS)
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
                
                if (item.hasBeenExecuted is false)
                {
                    if(_bufferType is BufferType.Hold)
                        if (!actionsStillHeld.Contains(item.gameAction))
                            return;
                    
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

        private void ProcessInput(GameTime gameTime) // if button inputs are read, executed associated actions
        {
            //List<GameAction> heldActions = _inputManager.GetKeyInput();
            _softDrop = _currentInputEvents.Contains(GameAction.SoftDrop);

            if (_currentInputEvents.Contains(GameAction.Hold)) // fucking gross
            {
                switch (_currentBoardState)
                {
                    case BoardState.Playing:
                        if (!_lastInputEvents.Contains(GameAction.Hold) && _pieceManager.holdEnabled && HoldPiece() is not null)
                        {
                            SfxBank.hold.Play();
                            currentSpinType = SpinType.None;
                            _softLockDelay.timeLeftover = _softLockDelay.max;
                        }
                        break;
                    default:
                        if(_bufferType is not BufferType.None)
                            preHoldRequested = true;
                        break;
                }
            }
            else
                    preHoldRequested = false;

            if (_currentInputEvents.Contains(GameAction.RotateCw))
            {
                switch (_currentBoardState)
                {
                    case BoardState.Playing:
                        if (!_lastInputEvents.Contains(GameAction.RotateCw))
                            Rotate(RotationType.Clockwise);
                        break;
                    default:
                        if (_bufferType is not BufferType.None)
                            preRotationType = RotationType.Clockwise;
                        break;
                }
            }
            else
            {
                if(preRotationType == RotationType.Clockwise)
                    preRotationType = null;
            }

            

            if (_currentInputEvents.Contains(GameAction.RotateCcw))
            {
                switch (_currentBoardState)
                {
                    case BoardState.Playing:
                        if (!_lastInputEvents.Contains(GameAction.RotateCcw))
                            Rotate(RotationType.CounterClockwise);
                        break;
                    default:
                        if (_bufferType is not BufferType.None)
                            preRotationType = RotationType.CounterClockwise;
                        break;
                }
            }
            else
            {
                if(preRotationType == RotationType.CounterClockwise)
                    preRotationType = null;
            }

            if (_currentInputEvents.Contains(GameAction.MovePieceLeft)&& !_lastInputEvents.Contains(GameAction.MovePieceLeft))
            {
                if(activePiece is not null)
                    MovePieceHorizontal(-1);
            }
            
            if (_currentInputEvents.Contains(GameAction.MovePieceRight) && !_lastInputEvents.Contains(GameAction.MovePieceRight))
            {
                if(activePiece is not null)
                    MovePieceHorizontal(1);
            }

            if (!_currentInputEvents.Contains(GameAction.HardDrop) ||
                _lastInputEvents.Contains(GameAction.HardDrop)) return;
            if (_hardDropCut.timeLeftover <= 0) 
            {
                if (_fastDropType is FastDropType.HardDrop)
                    HardDrop();
                else if (_fastDropType is FastDropType.FirmDrop)
                    FirmDrop();
            }
                
        }

        private void ChargeDas(GameTime gameTime) // if directional inputs are read, execute associated actions
        {
            List<GameAction> heldActions = _inputManager.GetKeyInputSelection(new Keys[] { _inputManager._binds.k_MovePieceLeft, _inputManager._binds.k_MovePieceRight });
            if (heldActions.Contains(GameAction.MovePieceLeft))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceLeft))
                    _eventTimeStamps.TryAdd(GameAction.MovePieceLeft, (float)gameTime.TotalGameTime.TotalSeconds);
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceRight, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceRight);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceLeft)) && activePiece is not null && _dasCut.timeLeftover <= 0)
                        AutoShift(gameTime, (HoriMovement)(-1));
                }
            }
            else 
            {
                _eventTimeStamps.Remove(GameAction.MovePieceLeft);
            }


            if (heldActions.Contains(GameAction.MovePieceRight))
            {
                if (!_lastInputEvents.Contains(GameAction.MovePieceRight))
                    _eventTimeStamps.TryAdd(GameAction.MovePieceRight, (float)gameTime.TotalGameTime.TotalSeconds);
                if (!(_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out var num)) ||
                    (_eventTimeStamps.TryGetValue(GameAction.MovePieceLeft, out num) && _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight) > num))
                {
                    _eventTimeStamps.Remove(GameAction.MovePieceLeft);
                    if (CanDas(gameTime, _eventTimeStamps.GetValueOrDefault(GameAction.MovePieceRight)) && activePiece is not null && _dasCut.timeLeftover <= 0)
                        AutoShift(gameTime, (HoriMovement)1);
                }
            }
            else 
            {
                _eventTimeStamps.Remove(GameAction.MovePieceRight);
            }
                

        }

        public void Update(GameTime gameTime)
        {
            GetInput(_inputDevice, PlayerIndex.One);
            ProcessInput(gameTime);
            _dasCut.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _hardDropCut.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            ChargeDas(gameTime);
            
            switch (_currentBoardState)
            {
                case BoardState.Playing:
                    GravitySoftDrop(gameTime);
                    _lockDelayAmount = MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1);
                    break;
                case BoardState.LineClearDelay:
                    //if(_bufferType != BufferType.None)
                        //_inputManager.BufferKeyInput(gameTime);
                    _lineClearDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_lineClearDelay.timeLeftover > 0) break;
                    DropLines();
                    _previousBoardState = BoardState.LineClearDelay;
                    _currentBoardState = BoardState.ArrivalDelay;
                    _arrivalDelay.timeLeftover = _arrivalDelay.max;
                    break;
                case BoardState.ArrivalDelay:
                    //if(_bufferType != BufferType.None)
                        //_inputManager.BufferKeyInput(gameTime);
                    _arrivalDelay.timeLeftover -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((_arrivalDelay.timeLeftover > 0)) break;
                    if (_nextAction == NextAction.Continue)
                        GrabNextPiece();
                    else
                        _currentBoardState = BoardState.EventPause;
                    break;
                case BoardState.TopOut:
                    var interval = .15f;
                    activePiece = null;
                    rowTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    grid.SetDrawMode();
                    if (_greyRow >= _highestRow && rowTime >= interval)
                    {
                        rowTime = 0;
                        grid.ColorRow(_greyRow, 8);
                        _greyRow--;
                    }
                    if (_greyRow < _highestRow)
                        GameEnd?.Invoke();
                    break;
                case BoardState.Finish:
                    var intervalB = .15f;
                    activePiece = null;
                    rowTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    grid.SetDrawMode();
                    if (_greyRow >= _highestRow && rowTime >= intervalB)
                    {
                        rowTime = 0;
                        for (int i = 0; i < Grid.COLUMNS; i++)
                        {
                            if (grid._matrix[_greyRow][i] != 0 && !grid.rowsToClear.Contains(_greyRow))
                                AnimatedEffectManager.AddEffect(new LockFlash(ImgBank.BlockTexture, grid.imageTiles[grid._matrix[_greyRow][i] - 1], new Vector2(offset.X + (i * 8), offset.Y + (_greyRow * 8) - 160), Color.White, 1f, Vector2.Zero));
                        }
                        grid.ColorRow(_greyRow, 0);
                        _greyRow--;
                    }
                    if (_greyRow < _highestRow)
                        GameEnd?.Invoke();
                    break;
            }

             _lastInputEvents = _inputDevice switch
             {
                 InputDevice.Gamepad => _inputManager.GetButtonInput(PlayerIndex.One),
                 InputDevice.Keyboard => _inputManager.GetKeyInput(),
                 _ => _inputManager.GetKeyInput()
             };
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !_prevKbState.IsKeyDown(Keys.T))
                _showDebug = !_showDebug;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && !_prevKbState.IsKeyDown(Keys.Y))
                _temporaryLandingSys = !_temporaryLandingSys;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !_prevKbState.IsKeyDown(Keys.R))
                grid.ClearGrid();
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !_prevKbState.IsKeyDown(Keys.G)) 
            {
                if (activePiece is not null && activePiece.offsetY == CalculateGhostPiece(activePiece) && activePiece.offsetY > -1) 
                    activePiece.offsetY--;
                grid.AddGarbageLine(8);
            }
            */
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && !_prevKbState.IsKeyDown(Keys.Tab))
                activePiece = _pieceManager.DealPiece(null, false);
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !_prevKbState.IsKeyDown(Keys.A))
                grid.LockData(TestGridFormations.test_TSD, Grid.ROWS - TestGridFormations.test_TSD.GetLength(0), 0);
            if (Keyboard.GetState().IsKeyDown(Keys.S) && !_prevKbState.IsKeyDown(Keys.S))
                grid.LockData(TestGridFormations.test_TST, Grid.ROWS - TestGridFormations.test_TST.GetLength(0), 0);
            if (Keyboard.GetState().IsKeyDown(Keys.D) && !_prevKbState.IsKeyDown(Keys.D))
                grid.LockData(TestGridFormations.test_SST, Grid.ROWS - TestGridFormations.test_SST.GetLength(0), 0);
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !_prevKbState.IsKeyDown(Keys.F))
                grid.LockData(TestGridFormations.test_SSD, Grid.ROWS - TestGridFormations.test_SSD.GetLength(0), 0);
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !_prevKbState.IsKeyDown(Keys.G))
                grid.LockData(TestGridFormations.test_STSD, Grid.ROWS - TestGridFormations.test_STSD.GetLength(0), 0);
            _prevKbState = Keyboard.GetState();
#endif

            if (activePiece is not null) 
            {
                
                _prevXOff = activePiece.offsetX;
            }
                
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
                            spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + (piece.offsetX * 8) + (int)offset.X, (y * 8) + (CalculateGhostPiece(activePiece) * 8) + (int)offset.Y - 160, 8, 8), grid.imageTiles[7], Color.White * .35f);
                        spriteBatch.Draw(ImgBank.BlockTexture, new Rectangle((x * 8) + (piece.offsetX * 8) + (int)offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), grid.imageTiles[piece.currentRotation[y, x] - 1], Color.Lerp(Color.DarkGray, Color.White, MathHelper.Clamp(_softLockDelay.timeLeftover / _softLockDelay.max, 0, 1)));
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

        private void DrawPieceDb(SpriteBatch spriteBatch, Piece piece) 
        {
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
                            spriteBatch.Draw(cornerImg, new Rectangle((x * 8) + (piece.offsetX * 8) + (int)offset.X, (y * 8) + (piece.offsetY * 8) + (int)offset.Y - 160, 8, 8), Color.White);
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
                if (activePiece is not null) 
                {
                    DrawPiece(spriteBatch, activePiece);
#if DEBUG
                    DrawPieceDb(spriteBatch, activePiece);
#endif
                }
            }
            _pieceManager.Draw(spriteBatch);
            
            if(_currentBoardState is not BoardState.TopOut && _spawnAreaObscured)
                DrawPieceDanger(spriteBatch, _pieceManager.pieceQueue.Peek());

# if DEBUG
            for (var y = 0; y < _pieceManager.spawnArea.Length; y++) 
            {
                for (var x = 0; x < _pieceManager.spawnArea[y].Length; x++) 
                    if(_showDebug) spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Block/spawnPt"), new Vector2(offset.X + (x * 8) + (_pieceManager.spawnAreaPosition.X * 8), offset.Y + (y * 8) + (_pieceManager.spawnAreaPosition.Y * 8) - 160), Color.White * .63f);
            }
#endif
            spriteBatch.End();
        }
    }
}
