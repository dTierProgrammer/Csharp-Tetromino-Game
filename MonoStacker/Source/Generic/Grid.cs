using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.Generic
{
    public class Grid // TODO: Refactor to jagged int array
    {
        private Microsoft.Xna.Framework.Vector2 _offset;
        
        public List<Rectangle> imageTiles = new();
        private const int TILESIZE = 8;

        private const int ROWS = 40; // y
        // Higher than needed to account for pieces not covering death zone but above accesible field (guideline compliant)
        private const int COLUMNS = 10; // x
        //private int[,] _matrix;
        private int[][] _matrix;

        public List<int> rowsToClear { get; private set; }

        private Texture2D grid = GetContent.Load<Texture2D>("Image/Board/grid");
        public Texture2D blocks = GetContent.Load<Texture2D>("Image/Block/0");
        public Texture2D ghostBlocks = GetContent.Load<Texture2D>("Image/Block/0gp");

        public Grid(Microsoft.Xna.Framework.Vector2 Position) 
        {
            rowsToClear = new();
            _offset = Position;
            //_matrix = new int[ROWS, COLUMNS];

            GetImageCuts();

            /*
            for (int y = 0; y < ROWS; y++) // row
            {
                for (int x = 0; x < COLUMNS; x++) // column
                {
                    _matrix[y, x] = 0;
                }
            }
            */

            _matrix = new int[ROWS][];

            for (int y = 0; y < ROWS; y++) 
            {
                _matrix[y] = new int[COLUMNS];

                for (int x = 0; x < COLUMNS; x++) 
                {
                    _matrix[y][x] = 0;
                }
            }
        }

        private void GetImageCuts() 
        {
            imageTiles.Add(new Rectangle(0, 0, TILESIZE, TILESIZE)); // I
            imageTiles.Add(new Rectangle(TILESIZE, 0, TILESIZE, TILESIZE)); // J
            imageTiles.Add(new Rectangle(TILESIZE * 2, 0, TILESIZE, TILESIZE)); // L
            imageTiles.Add(new Rectangle(TILESIZE * 3, 0, TILESIZE, TILESIZE)); // O
            imageTiles.Add(new Rectangle(TILESIZE * 4, 0, TILESIZE, TILESIZE)); // S
            imageTiles.Add(new Rectangle(TILESIZE * 5, 0, TILESIZE,  TILESIZE)); // T
            imageTiles.Add(new Rectangle(TILESIZE * 6, 0, TILESIZE, TILESIZE)); // Z
        }

        public void LockPiece(Piece piece, int rowOffset, int columnOffset) 
        {
            if (rowOffset > -2) 
            {
                for (int y = 0; y < piece.currentRotation.GetLength(0); y++) // row
                {
                    for (int x = 0; x < piece.currentRotation.GetLength(1); x++) // column
                    {
                        if (piece.currentRotation[y, x] != 0) 
                        {
                            _matrix[y + rowOffset][ x + columnOffset] = piece.currentRotation[y, x];
                        }
                    }
                }
            }
        }

        public bool IsPlacementValid(Piece piece, int rowOffset, int columnOffset) 
        {
            for (int y = 0; y < piece.currentRotation.GetLength(0); y++) // row
            {
                for (int x = 0; x < piece.currentRotation.GetLength(1); x++) // column
                {
                    if (piece.currentRotation[y, x] > 0)
                    {
                        /*
                        if (rowOffset + y < 0 || rowOffset + y > ROWS)
                            return false;
                        if (columnOffset + x < 0 || columnOffset + x > COLUMNS)
                            return false;
                        if (matrix[y + rowOffset, x + columnOffset] != 0)
                            return false;
                        */

                        if (rowOffset + y>= ROWS)
                            return false;
                        if (columnOffset + x < 0)
                            return false;
                        if (columnOffset + x >= COLUMNS)
                            return false;
                        //if (rowOffset + y < ROWS)
                            //continue;
                        if (_matrix[rowOffset + y][columnOffset + x] != 0)
                            return false;
                    }
                }
            }

            return true;
        }
        
        
        public int CheckForLines() 
        {
            rowsToClear.Clear();
            bool clearedLines = false;
            for (int y = 0; y < ROWS; y++) // row
            {
                clearedLines = true;
                for (int x = 0; x < COLUMNS; x++) // column
                {
                    if (_matrix[y][x] == 0) 
                    {
                        clearedLines = false;
                        break;
                    }
                }
                if (clearedLines == true) 
                {
                    rowsToClear.Add(y);
                }
                    
            }
            return rowsToClear.Count;
        }

        public bool IsLineEmpty(int rowIndex) 
        {
            bool isEmpty = true;
            Console.WriteLine(rowIndex);

            for (int i = 0; i < COLUMNS; i++) 
            {
                if (_matrix[rowIndex][i] != 0)
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }

        public bool IsLineFull(int rowIndex)
        {
            bool isFull = true;
            for (int i = 0; i < COLUMNS; i++)
            {
                if (_matrix[rowIndex][i] == 0)
                {
                    isFull = false;
                    break;
                }
            }
            return isFull;
        }

        public void ClearLine(int rowIndex) // fuck
        {
            for (int i = 0; i < COLUMNS; i++) 
            {
                _matrix[rowIndex][i] = 0;
            }
        }

        public void MoveRow(int rowIndex, int offset) 
        {
            for (int i = 0; i < COLUMNS; i++) 
            {
                _matrix[rowIndex + offset][i] = _matrix[rowIndex][i];
                _matrix[rowIndex][i] = 0;
            }
        }

        public void ClearLines() 
        {
            int clearedLines = 0;
            for (int y = ROWS - 1; y >= 0; y--) 
            {
                if (IsLineFull(y))
                {
                    ClearLine(y);
                    clearedLines++;
                }
                else if (clearedLines > 0) 
                {
                    MoveRow(y, clearedLines);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(grid, new Vector2(_offset.X, _offset.Y), Color.White);
            Rectangle sourceRect = imageTiles[0];

            for (int y = 0; y < ROWS; y++) 
            {
                for (int x = 0; x < COLUMNS; x++) 
                {
                    Color color = Color.LightGray;
                    if (y < 20)
                        color = Color.PaleVioletRed;
                    if (y == 0)
                        color = Color.Black;

                    switch (_matrix[y][x]) 
                    {
                        case 1:
                            sourceRect = imageTiles[0];
                            break;
                        case 2:
                            sourceRect = imageTiles[1];
                            break;
                        case 3:
                            sourceRect = imageTiles[2];
                            break;
                        case 4:
                            sourceRect = imageTiles[3];
                            break;
                        case 5:
                            sourceRect = imageTiles[4];
                            break;
                        case 6:
                            sourceRect = imageTiles[5];
                            break;
                        case 7:
                            sourceRect = imageTiles[6];
                            break;
                           
                    }

                    if (_matrix[y][x] != 0 && !rowsToClear.Contains(y)) 
                    {
                        spriteBatch.Draw
                                (
                                blocks,
                                new Rectangle((int)((x * TILESIZE) + _offset.X), (int)((y * TILESIZE) + _offset.Y - 160), TILESIZE, TILESIZE),
                                sourceRect,
                                color
                                );
                    }
                }
            }
        }
    }
}
