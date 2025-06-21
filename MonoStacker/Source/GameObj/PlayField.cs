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

namespace MonoStacker.Source.GameObj
{
    public class PlayField
    {
        private Vector2 _offset;
        Grid grid;

        KeyboardState prevKBState;
        public Piece activePiece { get; set; }
        Piece ghostPiece;
        Texture2D border = GetContent.Load<Texture2D>("Image/Board/generic_border_0");
        bool showGhostPiece = true;
        float lineClearDelay = .3f;
        float lockDelayMax = 5f;
        float lockDelay;
        float dasTimerL = .1f;
        float dasTimerR = .1f;
        bool showActivePiece = true;
        bool softDrop;
        float dropSpeed;
        public NextPreview nextPreview { get; private set; }
        public HoldPreview holdPreview { get; private set; }

        private Texture2D bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        private List<VisualEffect> _effectsList = new();

        public PlayField(Vector2 position)
        {
            _offset = position;
            grid = new Grid(_offset);
            lockDelay = lockDelayMax;
            nextPreview = new(new Vector2((border.Width) + _offset.X, _offset.Y), 6);
            activePiece = nextPreview.GetNextPiece();
        }

        public void Initialize() 
        {
            holdPreview = new(new Vector2(_offset.X - 28, _offset.Y), this);
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

        public void LineClearFlash(Color color, float timeDisplayed) 
        {
            for (int y = 0; y < 40; y++)
            {
                if (grid.rowsToClear.Contains(y))
                {
                    _effectsList.Add(new ClearFlash(new Vector2(((border.Width / 2) - 5) + _offset.X, (int)(y * 8) + _offset.Y - 157), color, timeDisplayed));
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
                    (int)(activePiece.offsetY + (activePiece is I ? SRSData.DataICW[testPt, i].Y : SRSData.DataJLSTZCW[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataICW[testPt, i].X : SRSData.DataJLSTZCW[testPt, i].X))) && activePiece is not O)
                {
                    Debug.WriteLine("true at " + i);
                    Console.WriteLine("true at " + i);
                    activePiece.RotateCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataICW[testPt, i].X : SRSData.DataJLSTZCW[testPt, i].X;
                    activePiece.offsetY += activePiece is I ? SRSData.DataICW[testPt, i].Y : SRSData.DataJLSTZCW[testPt, i].Y;
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
                    (int)(activePiece.offsetY + (activePiece is I ? SRSData.DataICCW[testPt, i].Y : SRSData.DataJLSTZCCW[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataICCW[testPt, i].X : SRSData.DataJLSTZCCW[testPt, i].X))) && activePiece is not O)
                {
                    Debug.WriteLine("true at " + i);
                    Console.WriteLine("true at " + i);
                    activePiece.RotateCCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataICCW[testPt, i].X : SRSData.DataJLSTZCCW[testPt, i].X;
                    activePiece.offsetY += activePiece is I ? SRSData.DataICCW[testPt, i].Y : SRSData.DataJLSTZCCW[testPt, i].Y;
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
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z) && showActivePiece)
            {
                RotateCCWSRS();
                activePiece.Update();
                Debug.WriteLine(activePiece.rotationId);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) && showActivePiece)
            {
                if (!prevKBState.IsKeyDown(Keys.Left))
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1))
                        activePiece.offsetX -= 1;
                }
                dasTimerL -= deltaTime;
                if (dasTimerL <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1))
                        activePiece.offsetX -= 1;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right) && showActivePiece)
            {
                if (!prevKBState.IsKeyDown(Keys.Right))
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX + 1))
                        activePiece.offsetX += 1;
                }
                dasTimerR -= deltaTime;
                if (dasTimerR <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX + 1))
                        activePiece.offsetX += 1;
                }
            }
            else 
            {
                dasTimerR = .2f;
                dasTimerL = .2f;
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
                if(lineClearDelay == .3f)
                    LineClearFlash(Color.White, .5f);
                lineClearDelay -= deltaTime;
                if (lineClearDelay <= 0)
                {
                    grid.ClearLines();
                    lineClearDelay = .3f;
                    activePiece = nextPreview.GetNextPiece();
                    showActivePiece = true;
                    
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
                    if (piece.currentRotation[y, x] != 0) 
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
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(bgTest, _offset, Color.White);
            grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 6, _offset.Y - 3), Color.White);
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
