using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.Generic
{
    enum DrawMode 
    {
        Playing,
        GameEnd
    }
    public class Grid // TODO: Refactor to jagged int array (I did this months ago, actually)
    {
        private Microsoft.Xna.Framework.Vector2 _offset;
        private DrawMode _drawMode = DrawMode.Playing;

        public readonly List<Rectangle> imageTiles = new();
        private const int TILESIZE = 8;

        public const int ROWS = 40; // y
        // Higher than needed to account for pieces not covering death zone but above accessible field (guideline compliant)
        public const int COLUMNS = 10; // x
        public int[][] _matrix { get; private set; }

        public List<int> rowsToClear { get; private set; }

        public Texture2D grid { get; set; } = GetContent.Load<Texture2D>("Image/Board/grid");
        public Texture2D blocks { get; set; } = GetContent.Load<Texture2D>("Image/Block/1");
        public Texture2D ghostBlocks { get; set; } = GetContent.Load<Texture2D>("Image/Block/0gp");

        public Texture2D debugCM = GetContent.Load<Texture2D>("Image/Block/cornerMandatory");
        public Texture2D debugCO = GetContent.Load<Texture2D>("Image/Block/cornerOptional");
        private Texture2D draw = GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect");

        public readonly Stack<int[]> overflowRows;

        bool drawLines = true;

        public Grid(Microsoft.Xna.Framework.Vector2 Position) 
        {
            rowsToClear = new();
            _offset = Position;
            overflowRows = new();

            GetImageCuts();

            _matrix = new int[ROWS][];

            for (int y = 0; y < ROWS; y++) 
            {
                _matrix[y] = new int[COLUMNS];

                for (int x = 0; x < COLUMNS; x++) 
                    _matrix[y][x] = 0;
            }
        }

        private void GetImageCuts() 
        {
            imageTiles.Add(new Rectangle(0, 0, TILESIZE, TILESIZE)); // Cyan
            imageTiles.Add(new Rectangle(TILESIZE, 0, TILESIZE, TILESIZE)); // Blue
            imageTiles.Add(new Rectangle(TILESIZE * 2, 0, TILESIZE, TILESIZE)); // Orange
            imageTiles.Add(new Rectangle(TILESIZE * 3, 0, TILESIZE, TILESIZE)); // Yellow
            imageTiles.Add(new Rectangle(TILESIZE * 4, 0, TILESIZE, TILESIZE)); // Green
            imageTiles.Add(new Rectangle(TILESIZE * 5, 0, TILESIZE,  TILESIZE)); // Purple/Magenta/Pink
            imageTiles.Add(new Rectangle(TILESIZE * 6, 0, TILESIZE, TILESIZE)); // Red
            imageTiles.Add(new Rectangle(0, TILESIZE, TILESIZE, TILESIZE));
            imageTiles.Add(new Rectangle(TILESIZE, TILESIZE, TILESIZE, TILESIZE));
            imageTiles.Add(new Rectangle(TILESIZE * 2, TILESIZE, TILESIZE, TILESIZE));
        }

        public void SetDrawMode() 
        {
            if(_drawMode != DrawMode.GameEnd)
                _drawMode = DrawMode.GameEnd;
        }

        public void LockPiece(Piece piece, int rowOffset, int columnOffset) 
        {
            if (rowOffset > -2) 
            {
                for (int y = 0; y < piece.currentRotation.GetLength(0); y++) // row
                {
                    for (int x = 0; x < piece.currentRotation.GetLength(1); x++) // column
                    {
                        if (piece.currentRotation[y, x] > 0) 
                            _matrix[y + rowOffset][ x + columnOffset] = piece.currentRotation[y, x];
                    }
                }
            }
        }

        public void LockData(int[,] data, int rowOffset, int columnOffset) 
        {
            for (int y = 0; y < data.GetLength(0); y++) 
            {
                for (int x = 0; x < data.GetLength(1); x++) 
                {
                    if (data[y, x] > 0) 
                        _matrix[y + rowOffset][x + columnOffset] = data[y, x];
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
                        if (rowOffset + y > ROWS)
                            continue;
                        if (_matrix[rowOffset + y][columnOffset + x] > 0)
                            return false;
                    }
                }
            }

            return true;
        }

        public bool IsDataPlacementValid(int[,] data, int rowOffset, int columnOffset) 
        {
            for (int y = 0; y < data.GetLength(0); y++) // row
            {
                for (int x = 0; x < data.GetLength(1); x++) // column
                {
                    if (data[y, x] > 0)
                    {
                        if (rowOffset + y >= ROWS) return false;
                        if (columnOffset + x < 0) return false;
                        if (columnOffset + x >= COLUMNS) return false;
                        if (rowOffset + y > ROWS) continue;
                        if (_matrix[rowOffset + y][columnOffset + x] > 0) return false;
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
                if (clearedLines) 
                    rowsToClear.Add(y);
                    
            }
            return rowsToClear.Count;
        }



        public SpinType CheckForSpin(Piece piece) // Z/S spin doubles aren't counted for whatever reason, investigate sometime
        {
            int mandatoryCornersFilled = 0;
            int optionalCornersFilled = 0;
            int totalCornersFilled = 0;

            for (int y = 0; y < piece.requiredCorners.GetLength(0); y++) // row
            {
                for (int x = 0; x < piece.requiredCorners.GetLength(1); x++) // column
                {
                    if (piece.requiredCorners[y, x] == 1)
                    {
                        if ((piece.offsetX + x >= COLUMNS || piece.offsetX + x < 0) ||
                            (piece.offsetY + y >= ROWS || piece.offsetY + y < 0) ||
                            _matrix[(int)piece.offsetY + y][(int)piece.offsetX + x] > 0)
                        { mandatoryCornersFilled++; totalCornersFilled++; }

                    }
                    
                    
                    if (piece.requiredCorners[y, x] == 2)
                    {
                        if ((piece.offsetX + x >= COLUMNS || piece.offsetX + x < 0) ||
                            (piece.offsetY + y >= ROWS || piece.offsetY + y < 0) ||
                            _matrix[(int)piece.offsetY + y][(int)piece.offsetX + x] > 0)
                        { optionalCornersFilled++; totalCornersFilled++; }
                    }
                    
                }
            }

            if (piece.type is TetrominoType.T)
            {
                switch (mandatoryCornersFilled) 
                {
                    case 1:
                        if (optionalCornersFilled >= 2)
                            return SpinType.MiniSpin;
                        break;
                    case 2:
                        if (optionalCornersFilled > 0)
                            return SpinType.FullSpin;
                        break;
                }
            }
            else
            {
                Debug.WriteLine(totalCornersFilled);
                switch (totalCornersFilled)
                {
                    case 0:
                        break;
                    case 1:
                        return SpinType.MiniSpin;
                    default:
                        return SpinType.FullSpin;
                }
            }


            return SpinType.None;
        }

        public int GetHighestRow() 
        {
            int row = 0;
            bool counting = true;
            for (var y = 0; y < ROWS; y++) 
            {
                row = y;
                for (var x = 0; x < COLUMNS; x++)
                {
                    if (_matrix[y][x] != 0)
                    { counting = false; break;}
                }
                if (!counting) break;
            }
            return row;
        }
        public bool IsLineEmpty(int rowIndex) 
        {
            bool isEmpty = true;
            Console.WriteLine(rowIndex);

            for (int i = 0; i < COLUMNS; i++) 
            {
                if (_matrix[rowIndex][i] > 0)
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

        public int GetNonEmptyRows() // iterate through board, if encounter row that has values > 0, add to a list, return lists count
        {
            List<int[]> nonEmptyRows = new();

            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLUMNS; x++) 
                {
                    if (_matrix[y][x] != 0) 
                    {
                        nonEmptyRows.Add(_matrix[y]);
                        break;
                    }
                }
            }
            return nonEmptyRows.Count();
        }

        public void MoveRow(int rowIndex, int offset) 
        {
            for (int i = 0; i < COLUMNS; i++) 
            {
                if (rowIndex + offset >= 40 || rowIndex + offset < 0) return;
                _matrix[rowIndex + offset][i] = _matrix[rowIndex][i];
                _matrix[rowIndex][i] = 0;
            }
        }

        public void CopyRow(int rowIndex, int offset)
        {
            for (int i = 0; i < COLUMNS; i++)
            {
                if (rowIndex + offset >= 40 || rowIndex + offset < 0) return;
                _matrix[rowIndex + offset][i] = _matrix[rowIndex][i];
            }
        }
        public void ColorRow(int rowIndex, int colorId) 
        {
            for (int i = 0; i < COLUMNS; i++) 
            {
                if (_matrix[rowIndex][i] != 0)
                    _matrix[rowIndex][i] = colorId;
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
                    MoveRow(y, clearedLines);
            }
            rowsToClear.Clear();
        }

        public void ClearGrid() 
        {
            for (var y = 0; y < ROWS; y++) 
            {
                for (var x = 0; x < COLUMNS; x++) 
                {
                    _matrix[y][x] = 0;
                }
            }
        }

        public void AddGarbageLine(int hole) 
        {
            if (GetNonEmptyRows() == ROWS - 1)
            {
                overflowRows.Push(_matrix[0]);
                ClearLine(0);
            }

            for (var y = 0; y < ROWS; y++)
                MoveRow(y, -1);
            for (var x = 0; x < COLUMNS; x++)
            {
                if (x != hole)
                    _matrix[ROWS - 1][x] = 8;
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            Rectangle sourceRect = imageTiles[0];

            for (int y = 0; y < ROWS; y++) 
            {
                for (int x = 0; x < COLUMNS; x++) 
                {
                    Color color = Color.DarkGray;
                    if (y < 20)
                        color = new Color(100, 100, 100);
                    if (y == 0)
                        color = Color.Black;
                    if (_matrix[y][x] > 0 && !rowsToClear.Contains(y)) 
                    {
                        
                        spriteBatch.Draw
                                (
                                ImgBank.BlockTexture,
                                new Rectangle((int)((x * TILESIZE) + _offset.X), (int)((y * TILESIZE) + _offset.Y - 160), TILESIZE, TILESIZE),
                                imageTiles[_matrix[y][x] - 1],
                                _drawMode is DrawMode.Playing? color: Color.White,
                                0,
                                Vector2.Zero,
                                SpriteEffects.None,
                                0
                                );
                        

                        if (drawLines && _drawMode is DrawMode.Playing) 
                        { // chopped ahh hell
                            if (y >= 19 && !(x - 1 < 0) && (_matrix[y][x - 1] == 0) ||
                                (y <= 19 && x == 0) ||
                                (y <= 19 && x > 0 && (_matrix[y][x - 1] == 0)))
                                spriteBatch.Draw(draw, new Rectangle((int)((x * TILESIZE) + _offset.X), (int)((y * TILESIZE) + _offset.Y - 160), 1, TILESIZE), Color.White);
                            if  ((y >= 19 && !(x + 1 >= COLUMNS) && _matrix[y][x + 1] == 0) ||
                                (y <= 19 && x == COLUMNS - 1) || 
                                (y <= 19 && x < COLUMNS - 1) && _matrix[y][x + 1] == 0)
                                spriteBatch.Draw(draw, new Rectangle((int)((x * TILESIZE) + _offset.X + 7), (int)((y * TILESIZE) + _offset.Y - 160), 1, TILESIZE), Color.White);
                            if (!(y - 1 < 0) && (_matrix[y - 1][x] == 0) || rowsToClear.Contains(y - 1))
                                spriteBatch.Draw(draw, new Rectangle((int)((x * TILESIZE) + _offset.X), (int)((y * TILESIZE) + _offset.Y - 160), TILESIZE, 1), Color.White);
                            if (!(y + 1 >= ROWS) && (_matrix[y + 1][x] == 0 || rowsToClear.Contains(y + 1)))
                                spriteBatch.Draw(draw, new Rectangle((int)((x * TILESIZE) + _offset.X), (int)((y * TILESIZE) + _offset.Y - 160 + 7), TILESIZE, 1), Color.White);
                        }
                    }
                }
            }
        }
    }
}
