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
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;
using Vector2 = Microsoft.Xna.Framework.Vector2;

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
            switch (activePiece.rotationId) 
            {
                case 0: // 1 || 0 -> R
                    testPt = 0;
                    break;
                case 1: // 2 || R -> 2
                    testPt = 2;
                    break; 
                case 2: // 3 || 2 -> L
                    testPt = 4;
                    break;
                case 3: // 0 || L -> 0
                    testPt = 6;
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                if
                    (grid.IsDataPlacementValid(activePiece.rotations[activePiece.ProjectRotateCW()],
                    (int)(activePiece.offsetY + (activePiece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X))) && activePiece is not O)
                {
                    activePiece.RotateCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X;
                    activePiece.offsetY += activePiece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y;
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
            switch (activePiece.rotationId) 
            {
                case 0: // 3 || 0 -> L
                    testPt = 7;
                    break;
                case 1: // 0 || R -> 0
                    testPt = 1;
                    break;
                case 2: // 1 || 2 -> R
                    testPt = 3;
                    break;
                case 3: // 2 || L -> 2
                    testPt = 5;
                    break;
            }

            for (int i = 0; i < 5; i++)
            {
                if
                    (grid.IsDataPlacementValid(activePiece.rotations[activePiece.ProjectRotateCCW()],
                    (int)(activePiece.offsetY + (activePiece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y)),
                    (int)(activePiece.offsetX + (activePiece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X))) && activePiece is not O)
                {
                    activePiece.RotateCCW();
                    activePiece.offsetX += activePiece is I ? SRSData.DataI[testPt, i].X : SRSData.DataJLSTZ[testPt, i].X;
                    activePiece.offsetY += activePiece is I ? SRSData.DataI[testPt, i].Y : SRSData.DataJLSTZ[testPt, i].Y;
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
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z) && showActivePiece)
            {
                RotateCCWSRS();
                activePiece.Update();
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
            {
                HardDrop();
            }

            prevKBState = Keyboard.GetState();

            if (grid.IsPlacementValid(activePiece, (int)(activePiece.offsetY + dropSpeed), (int)activePiece.offsetX))
            {
                activePiece.offsetY += dropSpeed;
            }
            else 
            {
                lockDelay -= deltaTime;

                /*
                if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1) ||
                    grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1)) // makes lock delay inconsisteny when no directional inputs are made...
                    lockDelay += deltaTime / 1.2f;
                */
                    if (lockDelay <= 0) 
                {
                    LockPiece();
                }
            }


            if (grid.CheckForLines() > 0)
            {
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
            spriteBatch.End();
            
        }
    }
}
