using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        Piece activePiece;
        Piece ghostPiece;
        Texture2D border = GetContent.Load<Texture2D>("Image/Board/generic_border_0");
        float lineClearDelay = .3f;
        float lockDelay = .3f;
        float dasTimerL = .1f;
        float dasTimerR = .1f;
        bool showActivePiece = true;
        bool softDrop;
        float dropSpeed;

        private Texture2D bgTest = GetContent.Load<Texture2D>("Image/Background/custombg_example_megurineluka");

        
        public PlayField(Vector2 position)
        {
            _offset = position;
            grid = new Grid(_offset);
            activePiece = _GenerateTetromino.RandomTetromino7Bag();
        }

        private void HardDrop()
        {
            activePiece.offsetY = CalculateGhostPiece();
            LockPiece();
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
                activePiece = _GenerateTetromino.RandomTetromino7Bag();
                showActivePiece = true;
            }
            lockDelay = .3f;
        }

        public void Update(float deltaTime) 
        {
            if (softDrop)
                dropSpeed = .5f;
            else
                dropSpeed = .08f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !prevKBState.IsKeyDown(Keys.Up) && showActivePiece)
            {
                activePiece.RotateCW();
                activePiece.Update();
                if (!(grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX)))
                {
                    Debug.WriteLine("false");
                    activePiece.RotateCCW();
                    activePiece.Update();
                }
                activePiece.Update();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z) && showActivePiece)
            {
                activePiece.RotateCCW();
                activePiece.Update();
                if (!(grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX))) 
                {
                    Debug.WriteLine("false");
                    activePiece.RotateCW();
                    activePiece.Update();
                }
                
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



            if (Keyboard.GetState().IsKeyDown(Keys.R) && !prevKBState.IsKeyDown(Keys.R))
                activePiece = _GenerateTetromino.RandomTetromino7Bag();




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

                if (grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1) ||
                    grid.IsPlacementValid(activePiece, (int)activePiece.offsetY, (int)activePiece.offsetX - 1)) // makes lock delay inconsisteny when no directional inputs are made...
                    lockDelay += deltaTime / 1.2f;

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
                    activePiece = _GenerateTetromino.RandomTetromino7Bag();
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

        public void DrawGhostPiece(SpriteBatch spriteBatch, Piece piece)
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
                        spriteBatch.Draw(
                                grid.ghostBlocks,
                                new Rectangle((x * 8) + ((int)piece.offsetX * 8) + (int)_offset.X, (y * 8) + (CalculateGhostPiece() * 8) + (int)_offset.Y - 160, 8, 8),
                                sourceRect,
                                Color.DarkGray
                                );
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            spriteBatch.Draw(bgTest, _offset, Color.White);
            grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 6, _offset.Y - 3), Color.DarkGray);
            if (showActivePiece) 
            {
                DrawGhostPiece(spriteBatch, activePiece);
                DrawPiece(spriteBatch, activePiece);
            }
                
            spriteBatch.End();
            
        }
    }
}
