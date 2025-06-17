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
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoStacker.Source.GameObj
{
    public class PlayField
    {
        private Vector2 _offset;
        Grid grid;

        KeyboardState prevKBState;
        Piece activePiece;
        //int offsetX, offsetY;

        
        public PlayField(Vector2 position)
        {
            _offset = position;
            grid = new Grid(_offset);
            activePiece = new I();
        }

        public void Update() 
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
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && !prevKBState.IsKeyDown(Keys.Left)) 
            {
                if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX - 1))
                    activePiece.offsetX -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && !prevKBState.IsKeyDown(Keys.Right))
            {
                if (grid.IsPlacementValid(activePiece, activePiece.offsetY, activePiece.offsetX + 1))
                    activePiece.offsetX += 1;
            }
            Debug.WriteLine(activePiece.offsetX);
            prevKBState = Keyboard.GetState();
        }

        public void DrawActivePiece(SpriteBatch spriteBatch) 
        {
            for (int y = 0; y < activePiece.currentRotation.GetLength(0); y++) 
            {
                for (int x = 0; x < activePiece.currentRotation.GetLength(1); x++) 
                {
                    switch (activePiece.currentRotation[y, x]) 
                    {
                        case 1:
                            spriteBatch.Draw(
                                grid.blocks, 
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8), 
                                grid.imageTiles[0], 
                                Color.White
                                );
                            break;
                        case 2:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
                                grid.imageTiles[1],
                                Color.White
                                );
                            break;
                        case 3:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
                                grid.imageTiles[2],
                                Color.White
                                );
                            break;
                        case 4:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
                                grid.imageTiles[3],
                                Color.White
                                );
                            break;
                        case 5:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
                                grid.imageTiles[4],
                                Color.White
                                );
                            break;
                        case 6:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
                                grid.imageTiles[5],
                                Color.White
                                );
                            break;
                        case 7:
                            spriteBatch.Draw(
                                grid.blocks,
                                new Rectangle((x * 8) + (activePiece.offsetX * 8) + (int)_offset.X, (y * 8) + (activePiece.offsetY * 8) + (int)_offset.Y, 8, 8),
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
            grid.Draw(spriteBatch);
            spriteBatch.Begin();
            DrawActivePiece(spriteBatch);
            spriteBatch.End();
        }
    }
}
