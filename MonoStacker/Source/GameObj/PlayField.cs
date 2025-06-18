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
        float timer = .3f;
        float dasTimerL = .1f;
        float dasTimerR = .1f;
        bool showActivePiece = true;

        
        public PlayField(Vector2 position)
        {
            _offset = position;
            grid = new Grid(_offset);
            activePiece = _GenerateTetromino.RandomTetromino();
            //ghostPiece = new I();
        }

        public bool SoftDrop() 
        {
            if (grid.IsPlacementValid(activePiece, activePiece.offsetY + 1, activePiece.offsetX)) 
            {
                activePiece.offsetY++;
                return true;
            }
            grid.LockPiece(activePiece, activePiece.offsetY, activePiece.offsetX);
            showActivePiece = false;
            if (grid.CheckForLines() == 0)
            {
                activePiece = _GenerateTetromino.RandomTetromino();
                showActivePiece = true;
            }
            ghostPiece = activePiece;
            return false;
        }

        public void HardDrop() 
        {
            activePiece.offsetY = CalculateGhostPiece();
            grid.LockPiece(activePiece, activePiece.offsetY, activePiece.offsetX);
            showActivePiece = false;
            if (grid.CheckForLines() == 0) 
            {
                activePiece = _GenerateTetromino.RandomTetromino();
                showActivePiece = true;
            }
                
            ghostPiece = activePiece;
        }

        public int CalculateGhostPiece() 
        {
            int xOff = activePiece.offsetX;
            int yOff = activePiece.offsetY;
            while (grid.IsPlacementValid(activePiece, yOff + 1, xOff))
            {
                yOff++;
            }
            return yOff;
        }

        public void Update(float deltaTime) 
        {
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !prevKBState.IsKeyDown(Keys.Up)) 
            {
                activePiece.RotateCW();
                activePiece.Update();
                if (!(grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX))) 
                {
                    Debug.WriteLine("false");
                    activePiece.RotateCCW();
                    activePiece.Update();
                }
                activePiece.Update();  
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !prevKBState.IsKeyDown(Keys.Z))
            {
                activePiece.RotateCCW();
                activePiece.Update();
                if (!(grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX))) 
                {
                    Debug.WriteLine("false");
                    activePiece.RotateCW();
                    activePiece.Update();
                }
                
            }



            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (!prevKBState.IsKeyDown(Keys.Left))
                {
                    if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX - 1))
                        activePiece.offsetX -= 1;
                }
                dasTimerL -= deltaTime;
                if (dasTimerL <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX - 1))
                        activePiece.offsetX -= 1;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (!prevKBState.IsKeyDown(Keys.Right))
                {
                    if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX + 1))
                        activePiece.offsetX += 1;
                }
                dasTimerR -= deltaTime;
                if (dasTimerR <= 0)
                {
                    if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX + 1))
                        activePiece.offsetX += 1;
                }
            }
            else 
            {
                dasTimerR = .2f;
                dasTimerL = .2f;
            }




            if (Keyboard.GetState().IsKeyDown(Keys.R) && !prevKBState.IsKeyDown(Keys.R))
                activePiece = _GenerateTetromino.RandomTetromino();




            if (Keyboard.GetState().IsKeyDown(Keys.Down)) 
            {
                SoftDrop();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space))
            {
                HardDrop();
            }

            prevKBState = Keyboard.GetState();

            
            if (grid.CheckForLines() > 0) 
            {
                timer -= deltaTime;
                if (timer <= 0) 
                {
                    grid.ClearLines();
                    timer = .3f;
                    activePiece = _GenerateTetromino.RandomTetromino();
                    showActivePiece = true;
                }
                    
            }
        }

        public void DrawPiece(SpriteBatch spriteBatch, Piece piece) 
        {
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++) 
                {
                    switch (piece.currentRotation[y, x]) 
                    {
                        case 1:
                            spriteBatch.Draw(
                                grid.blocks, 
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8), 
                                grid.imageTiles[0], 
                                Color.White
                                );
                            break;
                        case 2:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[1],
                                Color.White
                                );
                            break;
                        case 3:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[2],
                                Color.White
                                );
                            break;
                        case 4:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[3],
                                Color.White
                                );
                            break;
                        case 5:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[4],
                                Color.White
                                );
                            break;
                        case 6:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[5],
                                Color.White
                                );
                            break;
                        case 7:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (piece.offsetX * 8) + (int)_offset.X, (y * 8) + (piece.offsetY * 8) + (int)_offset.Y - 160, 8, 8),
                                grid.imageTiles[6],
                                Color.White
                                );
                            break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            grid.Draw(spriteBatch);
            spriteBatch.Draw(border, new Vector2(_offset.X - 6, _offset.Y - 3), Color.DarkGray);
            if(showActivePiece)
                DrawPiece(spriteBatch, activePiece);
            spriteBatch.End();
            
        }
    }
}
