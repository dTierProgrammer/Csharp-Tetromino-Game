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

    public class PlayField
    {
        private Vector2 _offset;
        private Grid _grid;

        KeyboardState prevKBState;
        
        public Piece activePiece { get; set; }
        private Color activePieceColor = Color.White;
        Texture2D border = GetContent.Load<Texture2D>("Image/Board/border");
        bool showGhostPiece = true;
        public float lineClearDelayMax { get; set; } = .5f;
        private float lineClearDelay;
        public float pieceEntryDelayMax { get; set; } = 0.06f;
        private float pieceEntryDelay;

        public float lockDelayMax { get; set; } = .63f;
        float lockDelay;
        public int lockDelayResetMovementMax { get; set; } = 15;
        public int lockDelayResetRotateMax { get; set; } = 6;
        int lockDelayResetMovement { get; set; }
        int lockDelayResetRotate { get; set; }

        const float maxDasTime = .14f;
        float dasTimerL = maxDasTime;
        float dasTimerR = maxDasTime;
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
        public SpinDenotation parsedSpins = SpinDenotation.AllSpin;

        private Texture2D bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private List<VisualEffect> _effectsList = new();

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
            dasTimerL = maxDasTime;
            dasTimerR = maxDasTime;
            pieceEntryDelay = pieceEntryDelayMax;
            nextPreview = new(new Vector2((border.Width) + _offset.X + 2, _offset.Y), 1);
            activePiece = nextPreview.GetNextPiece();
            activePiece.offsetY = 20;
            activePiece.offsetX = 3;
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
                movePiece.Play();

                if ((int)activePiece.offsetY == (int)CalculateGhostPiece())
                {
                    ResetLockDelayMovement();
                }
            }
        }

        private bool MoveDelayedAutoShift(ShiftDirection direction, float time)
        {
            if (time == 0)
            {
                if((int)direction == -1)
                    MovePiece(-1);
                
                if((int)direction == 1)
                    MovePiece(1);
            }
            else if (time >= maxDasTime)
            {
                if((int)direction == -1)
                    MovePiece(-1);
                if((int)direction == 1)
                    MovePiece(1);
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

            for (int i = 0; i < 5; i++)
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
                        ResetLockDelayRotate();
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

        private void LockPiece(float deltaTime)
        {
            activePieceColor = Color.White;
            ResetLockDelay();
            _grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            isPieceActive = false;
            Console.WriteLine(pieceEntryDelay);
            if (_grid.CheckForLines() == 0)
            {
                if (pieceEntryDelay == pieceEntryDelayMax)
                    FlashPiece(activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f : .3f);

                pieceEntryDelay -= deltaTime;

                if (pieceEntryDelay <= 0)
                {
                    activePiece = nextPreview.GetNextPiece();
                    isPieceActive = true;
                    lockPiece.Play();
                    pieceEntryDelay = pieceEntryDelayMax;
                }
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

        private void GravitySoftDrop(float deltaTime) 
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
                lockDelay -= deltaTime;
                
                /*
                activePieceColor.R -= (byte)(lockDelay * 17);
                activePieceColor.G -= (byte)(lockDelay * 17);
                activePieceColor.B -= (byte)(lockDelay * 17);
                */

                activePieceColor.R = (byte)Single.Lerp(activePieceColor.R, 50, (lockDelayMax - lockDelay) / 4);
                activePieceColor.G = (byte)Single.Lerp(activePieceColor.G, 50, (lockDelayMax - lockDelay) / 4);
                activePieceColor.B = (byte)Single.Lerp(activePieceColor.B, 50, (lockDelayMax - lockDelay) / 4);
                
                if (lockDelay <= 0)
                    LockPiece(deltaTime);
            }
        }

        private void HardDrop(float deltaTime)
        {
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece(deltaTime);
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

        private void ClearFilledLines(float deltaTime) 
        {
            if (_grid.CheckForLines() > 0)
            {
                activePieceColor = Color.White;
                lockDelay = lockDelayMax;
                if (lineClearDelay == lineClearDelayMax)
                {
                    LineClearFlash(Color.White, .5f);
                    if (_grid.rowsToClear.Count() == 4)
                        bigClear.Play();
                    else
                        clear.Play();

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
                            FlashPiece(activePiece, Color.Magenta, .7f, new Vector2(.5f, .5f));
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
                            //b2bHit.Play();
                            applause.Play();
                        }
                    }
                    else
                    {
                        if (b2bIsActive)
                        {
                            b2bIsActive = false;
                            b2bStreak = 0;
                        }
                    }
                }
                lineClearDelay -= deltaTime;
                if (lineClearDelay <= 0)
                {
                    _grid.ClearLines();
                    lineClearDelay = lineClearDelayMax;
                    activePiece = nextPreview.GetNextPiece();
                    isPieceActive = true;
                    lineFall.Play();
                    currentSpinType = SpinType.None;

                }
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
            
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && !prevKBState.IsKeyDown(Keys.Up)&& isPieceActive)
                {
                    RotateSRS(activePiece, RotationType.Clockwise);
                    //activePiece.BufferInputs();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z)&& isPieceActive)
                {
                    RotateSRS(activePiece, RotationType.CounterClockwise);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left)&& isPieceActive)
                {
                    if (!prevKBState.IsKeyDown(Keys.Left))
                        MovePiece(-1);
                    dasTimerL -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (dasTimerL <= 0)
                        MovePiece(-1);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right)&& isPieceActive)
                {
                    if (!prevKBState.IsKeyDown(Keys.Right))
                        MovePiece(1);
                    dasTimerR -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (dasTimerR <= 0)
                        MovePiece(1);
                }
                else 
                {
                        dasTimerR = maxDasTime;
                        dasTimerL = maxDasTime;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !prevKBState.IsKeyDown(Keys.LeftShift) && isPieceActive)
                    holdPreview.SwapPiece();

                if (Keyboard.GetState().IsKeyDown(Keys.Down)&& isPieceActive)
                    softDrop = true;
                else 
                    softDrop = false;
            

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space)&& isPieceActive)
                    HardDrop((float)gameTime.ElapsedGameTime.TotalSeconds);
                
                
            
# if DEBUG
                if (Keyboard.GetState().IsKeyDown(Keys.T) && !prevKBState.IsKeyDown(Keys.T))
                        showDebug = !showDebug;
                if (Keyboard.GetState().IsKeyDown(Keys.Y) && !prevKBState.IsKeyDown(Keys.Y))
                        showGhostPiece = !showGhostPiece;
# endif
                prevKBState = Keyboard.GetState();
            
            
            
            GravitySoftDrop((float)gameTime.ElapsedGameTime.TotalSeconds);
            ClearFilledLines((float)gameTime.ElapsedGameTime.TotalSeconds);

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
            //spriteBatch.Draw(bgTest, _offset, Color.White);
            _grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 4, _offset.Y - 4), Color.White);
            if (isPieceActive)
                DrawPiece(spriteBatch, activePiece);
            nextPreview.Draw(spriteBatch);
            holdPreview.Draw(spriteBatch);
            foreach (var item in _effectsList)
                item.Draw(spriteBatch);
            spriteBatch.End();
            
        }
    }
}
