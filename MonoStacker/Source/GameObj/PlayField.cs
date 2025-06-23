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

    public class PlayField
    {
        private Vector2 _offset;
        Grid grid;

        KeyboardState prevKBState;
        public Piece activePiece { get; set; }
        Piece ghostPiece;
        Texture2D border = GetContent.Load<Texture2D>("Image/Board/border");
        bool showGhostPiece = true;
        const float lineClearDelayMax = .6f;
        float lineClearDelay = lineClearDelayMax;
        float lockDelayMax = 5f;
        float lockDelay;
        const float maxDasTime = .14f;
        float dasTimerL = maxDasTime;
        float dasTimerR = maxDasTime;
        bool showActivePiece = true;
        bool softDrop;
        float dropSpeed;
        public NextPreview nextPreview { get; private set; }
        public HoldPreview holdPreview { get; private set; }
        public bool b2bIsActive { get; private set; } = false;
        public int b2bStreak { get; private set; }
        public SpinType currentSpinType { get; private set; } = SpinType.None;

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

        public bool showDebug = false;

        public PlayField(Vector2 position)
        {
            _offset = position;
            grid = new Grid(_offset);
            lockDelay = lockDelayMax;
            nextPreview = new(new Vector2((border.Width) + _offset.X + 2, _offset.Y), 5);
            activePiece = nextPreview.GetNextPiece();
            activePiece.offsetY = 20;
            activePiece.offsetX = 3;
        }

        public void Initialize() 
        {
            holdPreview = new(new Vector2(_offset.X - 42, _offset.Y), this);
        }

        private void HardDrop()
        {
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece();
        }

        private void ResetPiece() { }

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

        public void FlashPiece(Piece piece, Color color, float timeDislplayed, Vector2 distortFactor)
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

        public void LineClearFlash(Color color, float timeDisplayed) 
        {
            for (int y = 0; y < 40; y++)
            {
                if (grid.rowsToClear.Contains(y))
                {
                    _effectsList.Add(new ClearFlash(new Vector2(((border.Width / 2) - 5) + _offset.X, (int)(y * 8) + _offset.Y - 157), color, timeDisplayed, new Vector2(3, 1)));
                }
            }
        }

        private int CalculateGhostPiece() 
        {
            int xOff = (int)activePiece.offsetX;
            int yOff = (int)activePiece.offsetY;
            while (grid.IsPlacementValid(activePiece, yOff + 1, xOff))
            {
                yOff++;
            }
            return yOff;
        }

        private void LockPiece() 
        {
            grid.LockPiece(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX);
            showActivePiece = false;
            if (grid.CheckForLines() == 0)
            {
                FlashPiece(activePiece.offsetY < 20 ? Color.Red : Color.White, activePiece.offsetY < 20 ? 4f: .3f);
                activePiece = nextPreview.GetNextPiece();
                showActivePiece = true;
                lockPiece.Play();
            }
            ResetLockDelay();
            holdPreview.canHold = true;
        }

        public void ResetLockDelay() 
        {
            lockDelay = lockDelayMax;
        }

        private bool RotateCWSRS() 
        {
            currentSpinType = SpinType.None;
            int testPt = 0;
            switch (activePiece.ProjectRotateCW()) 
            {
                case 0:
                    testPt = 3;
                    break;
                case 1:
                    testPt = 0;
                    break; 
                case 2: 
                    testPt = 1;
                    break;
                case 3:
                    testPt = 2;
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                if
                    (grid.IsDataPlacementValid(activePiece.rotations[activePiece.ProjectRotateCW()],
                    (int)(activePiece.offsetY - (activePiece is I ? SRSData.DataICW[testPt, i].Y : SRSData.DataJLSTZCW[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataICW[testPt, i].X : SRSData.DataJLSTZCW[testPt, i].X))) && activePiece is not O)
                {
                    Debug.WriteLine("true at " + i);
                    Console.WriteLine("true at " + i);
                    activePiece.RotateCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataICW[testPt, i].X : SRSData.DataJLSTZCW[testPt, i].X;
                    activePiece.offsetY -= activePiece is I ? SRSData.DataICW[testPt, i].Y : SRSData.DataJLSTZCW[testPt, i].Y;
                    if(activePiece is T) 
                        currentSpinType = grid.CheckForSpin(activePiece);

                    if (currentSpinType != SpinType.None)
                        pieceSpin.Play();
                    else
                        rotatePiece.Play();
                        return true;
                }
                else 
                {
                    Debug.WriteLine("false at " + i);
                    Console.WriteLine("false at " + i);
                }
                   
            }
            return false;
        }

        public bool RotateCCWSRS() 
        {
            currentSpinType = SpinType.None;
            int testPt = 0;
            switch (activePiece.ProjectRotateCCW()) 
            {
                case 0:
                    testPt = 0;
                    break;
                case 1:
                    testPt = 1;
                    break;
                case 2:
                    testPt = 2;
                    break;
                case 3:
                    testPt = 3;
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                if
                    (grid.IsDataPlacementValid(activePiece.rotations[activePiece.ProjectRotateCCW()],
                    (int)(activePiece.offsetY - (activePiece is I ? SRSData.DataICCW[testPt, i].Y : SRSData.DataJLSTZCCW[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataICCW[testPt, i].X : SRSData.DataJLSTZCCW[testPt, i].X))) && activePiece is not O)
                {
                    Debug.WriteLine("true at " + i);
                    Console.WriteLine("true at " + i);
                    activePiece.RotateCCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataICCW[testPt, i].X : SRSData.DataJLSTZCCW[testPt, i].X;
                    activePiece.offsetY -= activePiece is I ? SRSData.DataICCW[testPt, i].Y : SRSData.DataJLSTZCCW[testPt, i].Y;
                    if (activePiece is T)
                    {
                        currentSpinType = grid.CheckForSpin(activePiece);
                        if (currentSpinType == SpinType.MiniSpin || currentSpinType == SpinType.FullSpin)
                            pieceSpin.Play();
                        else
                            rotatePiece.Play();
                    }
                    else 
                    { rotatePiece.Play(); }



                        return true;
                }
                else 
                {
                    Debug.WriteLine("false at " + i);
                    Console.WriteLine("false at " + i);
                }
            }
            return false;
        }

        public void Update(float deltaTime) 
        {
            if (softDrop)
                dropSpeed = .5f;
            else
                dropSpeed = .02f;
            nextPreview.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !prevKBState.IsKeyDown(Keys.Up) && showActivePiece)
            {
                RotateCWSRS();
                activePiece.Update();
                Debug.WriteLine(activePiece.rotationId);
                //rotatePiece.Play();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z) && showActivePiece)
            {
                RotateCCWSRS();
                activePiece.Update();
                Debug.WriteLine(activePiece.rotationId);
                //rotatePiece.Play();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) && showActivePiece)
            {
                if (!prevKBState.IsKeyDown(Keys.Left))
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1)) 
                    {
                        activePiece.offsetX -= 1;
                        movePiece.Play();
                    }
                }
                dasTimerL -= deltaTime;
                if (dasTimerL <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1))
                    {
                        activePiece.offsetX -= 1;
                        movePiece.Play();
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right) && showActivePiece)
            {
                if (!prevKBState.IsKeyDown(Keys.Right))
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX + 1)) 
                    {
                        activePiece.offsetX += 1;
                        movePiece.Play();
                    }
                        
                }
                dasTimerR -= deltaTime;
                if (dasTimerR <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX + 1))
                    {
                        activePiece.offsetX += 1;
                        movePiece.Play();
                    }
                }
            }
            else 
            {
                dasTimerR = maxDasTime;
                dasTimerL = maxDasTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !prevKBState.IsKeyDown(Keys.LeftShift) && showActivePiece)
                holdPreview.SwapPiece();

            if (Keyboard.GetState().IsKeyDown(Keys.Down) && showActivePiece)
                softDrop = true;
            else 
                softDrop = false;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space) && showActivePiece)
                HardDrop();

            if (Keyboard.GetState().IsKeyDown(Keys.T) && !prevKBState.IsKeyDown(Keys.T) && showActivePiece)
                _effectsList.Add(new ClearFlash(new Vector2(100, 100), Color.Red, .5f));

            prevKBState = Keyboard.GetState();

            if (grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + dropSpeed), (int)activePiece.offsetX))
                activePiece.offsetY += dropSpeed;

            else 
            {
                lockDelay -= deltaTime;
                if (lockDelay <= 0) 
                    LockPiece();
            }

            foreach (var item in _effectsList) 
                item.Update(deltaTime);
            _effectsList.RemoveAll(item => item.TimeDisplayed < 0);

            if (grid.CheckForLines() > 0)
            {
                if (lineClearDelay == lineClearDelayMax)
                {
                    LineClearFlash(Color.White, .5f);
                    if (grid.rowsToClear.Count() == 4)
                        bigClear.Play();
                    else
                        clear.Play();

                    if (currentSpinType != SpinType.None) // fake switch case
                    {
                        if (activePiece is I)
                            FlashPiece(activePiece, Color.Cyan, .5f, new Vector2(.5f, .5f));
                        if (activePiece is J)
                            FlashPiece(activePiece, Color.RoyalBlue, .5f, new Vector2(.5f, .5f));
                        if (activePiece is L)
                            FlashPiece(activePiece, Color.Orange, .5f, new Vector2(.5f, .5f));
                        if (activePiece is O)
                            FlashPiece(activePiece, Color.Yellow, .5f, new Vector2(.5f, .5f));
                        if (activePiece is S)
                            FlashPiece(activePiece, Color.Green, .5f, new Vector2(.5f, .5f));
                        if (activePiece is T)
                            FlashPiece(activePiece, Color.Magenta, .5f, new Vector2(.5f, .5f));
                        if (activePiece is Z)
                            FlashPiece(activePiece, Color.Red, .5f, new Vector2(.5f, .5f));
                    }

                    if (grid.rowsToClear.Count() == 4 || (activePiece is T && (currentSpinType == SpinType.FullSpin || currentSpinType == SpinType.MiniSpin)))
                    {
                        //applause.Play();
                        if (!b2bIsActive)
                        {
                            b2bIsActive = true;
                            b2bStreak = 0;
                        }
                        else
                        {
                            b2bStreak++;
                            //b2bHit.Play();
                            b2b.Play();
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
                    grid.ClearLines();
                    lineClearDelay = lineClearDelayMax;
                    activePiece = nextPreview.GetNextPiece();
                    showActivePiece = true;
                        
                    lineFall.Play();
                    
                            
                    currentSpinType = SpinType.None;
                        
                }
            }
        }

        public void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            Rectangle sourceRect = grid.imageTiles[0];
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++) 
                {
                    switch (piece.currentRotation[y, x]) 
                    {
                        case 1:
                            sourceRect = grid.imageTiles[0];
                            break;
                        case 2:
                            sourceRect = grid.imageTiles[1];
                            break;
                        case 3:
                            sourceRect = grid.imageTiles[2];
                            break;
                        case 4:
                            sourceRect = grid.imageTiles[3];
                            break;
                        case 5:
                            sourceRect = grid.imageTiles[4];
                            break;
                        case 6:
                            sourceRect = grid.imageTiles[5];
                            break;
                        case 7:
                            sourceRect = grid.imageTiles[6];
                            break;
                    }
                    if (piece.currentRotation[y, x] > 0) 
                    {
                        if (showGhostPiece)
                        {
                            spriteBatch.Draw(
                                    grid.ghostBlocks,
                                    new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + (CalculateGhostPiece() * 8) + (int)_offset.Y - 160, 8, 8),
                                    sourceRect,
                                    Color.White * .5f
                                    );
                        }

                        spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + ((int)piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                sourceRect,
                                Color.White
                                );
                    }
                   
                }
            }
            Texture2D cornerImg = grid.debugCM;
            if (showActivePiece && showDebug) 
            {
                for (int y = 0; y < piece.requiredCorners.GetLength(0); y++)
                {
                    for (int x = 0; x < piece.requiredCorners.GetLength(1); x++)
                    {
                        switch (piece.requiredCorners[y, x])
                        {
                            case 1:
                                cornerImg = grid.debugCM;
                                break;
                            case 2:
                                cornerImg = grid.debugCO;
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
            grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 4, _offset.Y - 4), Color.White);
            if (showActivePiece)
                DrawPiece(spriteBatch, activePiece);
            nextPreview.Draw(spriteBatch);
            holdPreview.Draw(spriteBatch);
            foreach (var item in _effectsList)
                item.Draw(spriteBatch);
            spriteBatch.End();
            
        }
    }
}
