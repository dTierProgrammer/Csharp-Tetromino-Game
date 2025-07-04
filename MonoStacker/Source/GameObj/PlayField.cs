using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Data;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Data;
using Microsoft.Xna.Framework.Audio;
using MonoStacker.Source.Interface.Input;

namespace MonoStacker.Source.GameObj
{
    public enum SpinType 
    {
        None,
        FullSpin,
        MiniSpin
    }

    public enum SpinDenotation 
    {
        None,
        TSpinOnly,
        TSpinSpecific,
        AllSpin
    }

    public enum RotationType
    {
        Clockwise = 0,
        CounterClockwise = 1
        //Flip = 2
    }

    public enum ShiftDirection
    {
        Left = -1,
        Right = 1
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
        private Vector2 _offset;
        private Grid _grid;

        KeyboardState prevKBState;
        InputManager _inputManager;

        BoardState currentBoardState = BoardState.Neutral;
        BoardState[] noDrawStates = { BoardState.LineClearWait, BoardState.PieceEntryWait, BoardState.GameEnd};
        
        public Piece activePiece { get; set; }
        private Color activePieceColor = Color.White;
        Texture2D border = GetContent.Load<Texture2D>("Image/Board/border");
        bool showGhostPiece = true;
        float waitTime = 0;
        public float lineClearDelayMax { get; set; } = .3f;
        private float lineClearDelay;
        public float pieceEntryDelayMax { get; set; } = .0f;
        private float pieceEntryDelay;

        public float lockDelayMax { get; set; } = .63f;
        float lockDelay;
        public int lockDelayResetMovementMax { get; set; } = 15;
        public int lockDelayResetRotateMax { get; set; } = 6;
        int lockDelayResetMovement { get; set; }
        int lockDelayResetRotate { get; set; }

        const float maxDasTime = .14f;
        private bool isPieceActive = true;
        private bool softDrop;
        private float dropSpeed;
        public float softDropSpeed { get; set; } = .5f;
        public NextPreview nextPreview { get; private set; }
        public int queueLength { get; private set; } = 5;
        public HoldPreview holdPreview { get; private set; }
        public bool b2bIsActive { get; private set; } = false;
        public int b2bStreak { get; private set; }
        public SpinType currentSpinType { get; private set; } = SpinType.None;
        public SpinDenotation parsedSpins = SpinDenotation.TSpinOnly;

        private Texture2D bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private List<VisualEffect> _effectsList = new();

        /*
        private SoundEffect movePiece = GetContent.Load<SoundEffect>("Audio/Sound/move");
        private SoundEffect lockPiece = GetContent.Load<SoundEffect>("Audio/Sound/lock");
        private SoundEffect rotatePiece = GetContent.Load<SoundEffect>("Audio/Sound/rotate");
        private SoundEffect clear = GetContent.Load<SoundEffect>("Audio/Sound/clear");
        private SoundEffect bigClear = GetContent.Load<SoundEffect>("Audio/Sound/clear0");
        private SoundEffect lineFall = GetContent.Load<SoundEffect>("Audio/Sound/linefall");
        private SoundEffect applause = GetContent.Load<SoundEffect>("Audio/Sound/s_hakushu");
        private SoundEffect b2b = GetContent.Load<SoundEffect>("Audio/Sound/b2b");
        private SoundEffect b2bHit = GetContent.Load<SoundEffect>("Audio/Sound/b2b_hit");
        private SoundEffect pieceSpin = GetContent.Load<SoundEffect>("Audio/Sound/spinrotate");
        */

        Dictionary<GameAction, float> eventTimeStamps = new();
        List<GameAction> lastEvents = new();
        

#if DEBUG
        public bool showDebug = true;
#else
        public bool showDebug = false;
#endif

        public PlayField(Vector2 position)
        {
            _offset = position;
            _grid = new Grid(_offset);
            lineClearDelay = lineClearDelayMax;
            lockDelay = lockDelayMax;
            lockDelayResetMovement = lockDelayResetMovementMax;
            lockDelayResetRotate = lockDelayResetRotateMax;
            pieceEntryDelay = pieceEntryDelayMax;
            nextPreview = new(new Vector2((border.Width) + _offset.X + 2, _offset.Y), 6);
            activePiece = nextPreview.GetNextPiece();
            activePiece.offsetY = 20;
            activePiece.offsetX = 3;
            _inputManager = new();
        }

        public void SetDelay(float delayAmt) 
        {

        }

        public void Initialize() 
        {
            holdPreview = new(new Vector2(_offset.X - 42, _offset.Y), this);
        }

        private void MovePiece(float movementAmt)
        {
            activePieceColor = Color.White;
            if (_grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)(activePiece.offsetX + movementAmt)))
            {
                activePiece.offsetX += movementAmt;
                SfxBank.stepHori.Play();

                if ((int)activePiece.offsetY == (int)CalculateGhostPiece())
                {
                    ResetLockDelayMovement();
                }
            }
        }

        private bool CanDas(GameTime gameTime, float timeStamp)
        {
            if ((float)(gameTime.TotalGameTime.TotalSeconds - timeStamp) >= maxDasTime) 
            {
                return true;
            }
                
            return false;
        }

        public bool RotateSRS(Piece piece, RotationType rotation)
        {
            currentSpinType = SpinType.None;
            int testPt = 0;
            if(piece is not O)
                testPt = (int)SRSData.GetSrsChecks(piece.rotationId, rotation == 0? piece.ProjectRotateCW(): piece.ProjectRotateCCW() );

            for (int i = 0; i < (piece is I ? SRSData.DataIArika.GetLength(1): SRSData.DataJLSTZ.GetLength(1)); i++)
            {
                if (_grid.IsDataPlacementValid(piece.rotations[rotation == 0 ? piece.ProjectRotateCW() : piece.ProjectRotateCCW()],
                    (int)(piece.offsetY - (piece is I ? SRSData.DataIArika[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y)),
                    (int)(piece.offsetX + (piece is I ? SRSData.DataIArika[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X))))
                {
                    switch (rotation)
                    {
                        case (RotationType)0: piece.RotateCW(); break;
                        case (RotationType)1: piece.RotateCCW(); break;
                    }

                    piece.offsetX += piece is I ? SRSData.DataIArika[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X;
                    piece.offsetY -= piece is I ? SRSData.DataIArika[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y;

                    switch (parsedSpins)
                    {
                        case SpinDenotation.TSpinOnly:
                            if (piece is T)
                                currentSpinType = _grid.CheckForSpin(piece);
                            break;
                        default:
                            currentSpinType = _grid.CheckForSpin(piece);
                            break;
                    }

                    if ((int)piece.offsetY == CalculateGhostPiece())
                    {
                        if (ResetLockDelayRotate())
                            activePieceColor = Color.White;
                    }
                    activePiece.Update();
                    return true;
                }
            }
            return false;
        }

        private int CalculateGhostPiece()
        {
            int xOff = (int)activePiece.offsetX;
            int yOff = (int)activePiece.offsetY;
            while (_grid.IsPlacementValid(activePiece, yOff + 1, xOff))
            {
                yOff++;
            }
            return yOff;
        }

        private void LockPiece(GameTime gameTime)
        {
            activePieceColor = Color.White;
            ResetLockDelay();
            _grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            if (_grid.CheckForLines() == 0) 
            {
                currentBoardState = BoardState.PieceEntryWait;
                pieceEntryDelay = pieceEntryDelayMax;
            }
            else if (_grid.CheckForLines() > 0) 
            {
                currentBoardState = BoardState.LineClearWait;
                lineClearDelay = lineClearDelayMax;
            }
            holdPreview.canHold = true;
        }

        public void ResetLockDelay()
        {
            lockDelay = lockDelayMax;
            lockDelayResetMovement = lockDelayResetMovementMax;
            lockDelayResetRotate = lockDelayResetRotateMax;
        }

        public bool ResetLockDelayRotate()
        {
            if (lockDelayResetRotate > 0)
            {
                lockDelayResetRotate--;
                lockDelay = lockDelayMax;

                return true;
            }
            return false;
        }

        public bool ResetLockDelayMovement()
        {
            if (lockDelayResetMovement > 0)
            {
                lockDelayResetMovement--;
                lockDelay = lockDelayMax;
                return true;
            }
            return false;
        }

        private void GravitySoftDrop(GameTime gameTime) 
        {
            if (softDrop)
                dropSpeed = .3f;
            else
                dropSpeed = .03f;

            if (activePiece.offsetY + dropSpeed <= CalculateGhostPiece())
            {
                if (_grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + dropSpeed), (int)activePiece.offsetX))
                {
                    activePiece.offsetY += dropSpeed;
                }
            }
            else
            {
                activePiece.offsetY = CalculateGhostPiece();
            }

            if ((int)activePiece.offsetY == (int)CalculateGhostPiece())
            {
                lockDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                activePieceColor.R = (byte)Single.Lerp(activePieceColor.R, 50, (lockDelayMax - lockDelay) / 4);
                activePieceColor.G = (byte)Single.Lerp(activePieceColor.G, 50, (lockDelayMax - lockDelay) / 4);
                activePieceColor.B = (byte)Single.Lerp(activePieceColor.B, 50, (lockDelayMax - lockDelay) / 4);

                if (lockDelay <= 0) 
                {
                    LockPiece(gameTime);
                    SfxBank.softLock.Play();
                }
                    
            }
        }

        private void HardDrop(GameTime gameTime)
        {
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece(gameTime);
            SfxBank.hardDrop.Play();
        }

        public void FlashPiece(Color color, float timeDislplayed) 
        {
            for (int y = 0; y < activePiece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < activePiece.currentRotation.GetLength(1); x++) 
                {
                    if (activePiece.currentRotation[y, x] != 0) 
                    {
                        _effectsList.Add(new LockFlash(new Vector2((x * 8) + ((int)activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)activePiece.offsetY * 8) + (int)_offset.Y - 160), color, timeDislplayed));
                    }
                }
            }
        }

        private void FlashPiece(Piece piece, Color color, float timeDislplayed, Vector2 distortFactor)
        {
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
                {
                    if (piece.currentRotation[y, x] != 0)
                    {
                        _effectsList.Add(new LockFlash(new Vector2((x * 8) + ((int)activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)activePiece.offsetY * 8) + (int)_offset.Y - 160), color, timeDislplayed, distortFactor));
                    }
                }
            }
        }

        private void ClearFilledLines() 
        {

            activePieceColor = Color.White;
            lockDelay = lockDelayMax;
            if (lineClearDelay == lineClearDelayMax)
            {
                LineClearFlash(Color.White, .5f);
                switch (_grid.rowsToClear.Count()) 
                {
                    case 1:
                        if (currentSpinType != SpinType.None) SfxBank.clearSpin[0].Play();
                        else SfxBank.clear[0].Play();
                        break;
                    case 2:
                        if (currentSpinType != SpinType.None) SfxBank.clearSpin[1].Play();
                        else SfxBank.clear[1].Play();
                        break;
                    case 3:
                        if (currentSpinType != SpinType.None) SfxBank.clearSpin[2].Play();
                        else SfxBank.clear[2].Play();
                        break;
                    default:
                        if (currentSpinType != SpinType.None) SfxBank.clearSpin[3].Play();
                        else SfxBank.clear[3].Play();
                        break;
                }

                if (currentSpinType != SpinType.None) // fake switch case
                {
                    if (activePiece is I)
                        FlashPiece(activePiece, Color.Cyan, .7f, new Vector2(.5f, .5f));
                    if (activePiece is J)
                        FlashPiece(activePiece, Color.RoyalBlue, .7f, new Vector2(.5f, .5f));
                    if (activePiece is L)
                        FlashPiece(activePiece, Color.Orange, .7f, new Vector2(.5f, .5f));
                    if (activePiece is O)
                        FlashPiece(activePiece, Color.Yellow, .7f, new Vector2(.5f, .5f));
                    if (activePiece is S)
                        FlashPiece(activePiece, Color.Green, .7f, new Vector2(.5f, .5f));
                    if (activePiece is T)
                        FlashPiece(activePiece, Color.Magenta, .7f, new Vector2(.5f, .8f));
                    if (activePiece is Z)
                        FlashPiece(activePiece, Color.Red, .7f, new Vector2(.5f, .5f));
                }

                if (_grid.rowsToClear.Count() == 4 || ((currentSpinType == SpinType.FullSpin || currentSpinType == SpinType.MiniSpin)))
                {
                    
                    if (!b2bIsActive)
                    {
                        b2bIsActive = true;
                        b2bStreak = 0;
                    }
                    else
                    {
                        b2bStreak++;
                        SfxBank.b2b.Play();
                    }
                }
                else
                {
                    if (b2bIsActive)
                    {
                        b2bIsActive = false;
                        b2bStreak = 0;
                        SfxBank.b2bBreak.Play();
                    }
                }
                currentSpinType = SpinType.None;
            }
        }

        private void LineClearFlash(Color color, float timeDisplayed) 
        {
            for (int y = 0; y < 40; y++)
            {
                if (_grid.rowsToClear.Contains(y))
                {
                    _effectsList.Add(new ClearFlash(new Vector2(((border.Width / 2) - 5) + _offset.X, (int)(y * 8) + _offset.Y - 157), color, timeDisplayed, new Vector2(3, 1)));
                }
            }
        }

        public void Update(GameTime gameTime) 
        {

            nextPreview.Update();

            switch (currentBoardState) 
            {
                case BoardState.Neutral:
                    if (_inputManager.bufferQueue.Count > 0)
                    {
                        List<InputEvent> bufferedActions = _inputManager.GetBufferedActions();
                        _inputManager.ClearBuffer();

                        foreach (var item in bufferedActions)
                        {
                            if (!item.hasBeenExecuted)
                            {
                                List<GameAction> actionsStillHeld = _inputManager.GetButtonInput();
                                if (item.gameAction == GameAction.Hold && actionsStillHeld.Contains(GameAction.Hold)) 
                                {
                                    if (holdPreview.SwapPiece())
                                        SfxBank.holdBuffer.Play();
                                }
                                    

                                if ((item.gameAction == GameAction.MovePieceLeft || item.gameAction == GameAction.MovePieceRight) &&
                                    (actionsStillHeld.Contains(GameAction.MovePieceLeft) || actionsStillHeld.Contains(GameAction.MovePieceRight)))
                                {
                                        if (!eventTimeStamps.ContainsKey(item.gameAction))
                                            eventTimeStamps.Add(item.gameAction, item.timePressed);
                                }

                                if (item.gameAction == GameAction.RotateCw && actionsStillHeld.Contains(GameAction.RotateCw)) 
                                {
                                    RotateSRS(activePiece, RotationType.Clockwise);
                                    SfxBank.rotateBuffer.Play();
                                }

                                if (item.gameAction == GameAction.RotateCcw && actionsStillHeld.Contains(GameAction.RotateCcw)) 
                                {
                                    RotateSRS(activePiece, RotationType.CounterClockwise);
                                    SfxBank.rotateBuffer.Play();
                                }
                                item.hasBeenExecuted = true;
                            }
                        }
                    }
                    else // if no inputs were buffered, get immediate inputs
                    {
                        List<GameAction> heldActions = _inputManager.GetButtonInput();

                        if (heldActions.Contains(GameAction.MovePieceLeft))
                        {
                            if (!lastEvents.Contains(GameAction.MovePieceLeft))
                            {
                                if (!eventTimeStamps.ContainsKey(GameAction.MovePieceLeft))
                                {
                                    eventTimeStamps.Add(GameAction.MovePieceLeft, (float)gameTime.TotalGameTime.TotalSeconds);
                                    MovePiece(-1);
                                }
                            }

                            if (eventTimeStamps.ContainsKey(GameAction.MovePieceLeft))
                            {
                                float timeStamp = eventTimeStamps[GameAction.MovePieceLeft];
                                if (CanDas(gameTime, timeStamp)) MovePiece(-1);
                            }
                        }
                        else
                        {
                            if (eventTimeStamps.ContainsKey(GameAction.MovePieceLeft))
                                eventTimeStamps.Remove(GameAction.MovePieceLeft);
                        }


                        if (heldActions.Contains(GameAction.MovePieceRight))
                        {
                            if (!lastEvents.Contains(GameAction.MovePieceRight))
                            {
                                if (!eventTimeStamps.ContainsKey(GameAction.MovePieceRight))
                                {
                                    eventTimeStamps.Add(GameAction.MovePieceRight, (float)gameTime.TotalGameTime.TotalSeconds);
                                    MovePiece(1);
                                }
                            }
                            if (eventTimeStamps.ContainsKey(GameAction.MovePieceRight))
                            {
                                float timeStamp = eventTimeStamps[GameAction.MovePieceRight];
                                if (CanDas(gameTime, timeStamp)) MovePiece(1);
                            }
                        }
                        else
                        {
                            if (eventTimeStamps.ContainsKey(GameAction.MovePieceRight))
                                eventTimeStamps.Remove(GameAction.MovePieceRight);
                        }

                        if (heldActions.Contains(GameAction.SoftDrop))
                            softDrop = true;
                        else
                            softDrop = false;

                        if (heldActions.Contains(GameAction.Hold) && !lastEvents.Contains(GameAction.Hold)) 
                        {
                            if (holdPreview.SwapPiece())
                                SfxBank.hold.Play();
                        }

                        if (heldActions.Contains(GameAction.RotateCw) && !lastEvents.Contains(GameAction.RotateCw)) 
                        {
                            RotateSRS(activePiece, RotationType.Clockwise);
                            SfxBank.rotate.Play();
                        }

                        if (heldActions.Contains(GameAction.RotateCcw) && !lastEvents.Contains(GameAction.RotateCcw)) 
                        {
                            RotateSRS(activePiece, RotationType.CounterClockwise);
                            SfxBank.rotate.Play();
                        }
                            
                        if (heldActions.Contains(GameAction.HardDrop) && !lastEvents.Contains(GameAction.HardDrop))
                            HardDrop(gameTime);
                    }
                    GravitySoftDrop(gameTime);
                    break;
                case BoardState.LineClearWait:
                    _inputManager.BufferButtonInput(gameTime);

                    // action
                    activePieceColor = Color.White;
                    lockDelay = lockDelayMax;

                    if (lineClearDelay == lineClearDelayMax)
                        ClearFilledLines();

                    lineClearDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (lineClearDelay <= 0)
                    {
                        _grid.ClearLines();
                        if (_grid.GetNonEmptyRows() > 0) SfxBank.lineFall.Play(); ;
                        activePiece = nextPreview.GetNextPiece();
                        activePieceColor = Color.White;
                        currentBoardState = BoardState.Neutral;
                        lineClearDelay = lineClearDelayMax;
                    }


                    break;
                case BoardState.PieceEntryWait:
                    _inputManager.BufferButtonInput(gameTime);

                    // action
                    if (pieceEntryDelay == pieceEntryDelayMax)
                    {
                        FlashPiece(activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f);
                        //lockPiece.Play();
                    }

                    pieceEntryDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (pieceEntryDelay <= 0) 
                    {
                        activePiece = nextPreview.GetNextPiece();
                        activePieceColor = Color.White;
                        currentBoardState = BoardState.Neutral;
                        pieceEntryDelay = pieceEntryDelayMax;
                    }
                    break;
            }
            lastEvents = _inputManager.GetButtonInput();

#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !prevKBState.IsKeyDown(Keys.T))
                showDebug = !showDebug;
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && !prevKBState.IsKeyDown(Keys.Y))
                        showGhostPiece = !showGhostPiece;
# endif
            prevKBState = Keyboard.GetState();
            
            

            foreach (var item in _effectsList)
                item.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _effectsList.RemoveAll(item => item.TimeDisplayed < 0);
        }

        public void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            Rectangle sourceRect = _grid.imageTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++) 
                {
                    switch (piece.currentRotation[y, x]) 
                    {
                        case 1:
                            sourceRect = _grid.imageTiles[0];
                            break;
                        case 2:
                            sourceRect = _grid.imageTiles[1];
                            break;
                        case 3:
                            sourceRect = _grid.imageTiles[2];
                            break;
                        case 4:
                            sourceRect = _grid.imageTiles[3];
                            break;
                        case 5:
                            sourceRect = _grid.imageTiles[4];
                            break;
                        case 6:
                            sourceRect = _grid.imageTiles[5];
                            break;
                        case 7:
                            sourceRect = _grid.imageTiles[6];
                            break;
                    }
                    if (piece.currentRotation[y, x] > 0) 
                    {
                        if (showGhostPiece)
                        {
                            spriteBatch.Draw(
                                    _grid.ghostBlocks,
                                    new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + (CalculateGhostPiece() * 8) + (int)_offset.Y - 160, 8, 8),
                                    sourceRect,
                                    Color.White * .5f
                                    );
                        }

                        spriteBatch.Draw(
                                _grid.blocks,
                                new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                sourceRect,
                                activePieceColor
                                );
                    }
                   
                }
            }
            Texture2D cornerImg = _grid.debugCM;
            if (isPieceActive && showDebug) 
            {
                for (int y = 0; y < piece.requiredCorners.GetLength(0); y++)
                {
                    for (int x = 0; x < piece.requiredCorners.GetLength(1); x++)
                    {
                        switch (piece.requiredCorners[y, x])
                        {
                            case 1:
                                cornerImg = _grid.debugCM;
                                break;
                            case 2:
                                cornerImg = _grid.debugCO;
                                break;
                            case 3:
                                cornerImg = GetContent.Load<Texture2D>("Image/Block/cenPt");
                                break;
                        }

                        if (piece.requiredCorners[y, x] > 0) 
                        {
                            spriteBatch.Draw(
                                cornerImg,
                                new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                Color.White
                                );
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bgTest, _offset, Color.White);
            _grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 4, _offset.Y - 4), Color.White);
            if(currentBoardState == BoardState.Neutral)
                DrawPiece(spriteBatch, activePiece);
            nextPreview.Draw(spriteBatch);
            holdPreview.Draw(spriteBatch);
            foreach (var item in _effectsList)
                item.Draw(spriteBatch);
            spriteBatch.End();
            
        }
    }
}
