using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoStacker.Source.GameObj.Tetromino;

namespace MonoStacker.Source.Generic
{
    public class Piece
    {
        public readonly List<int[,]> rotations = new();
        public readonly List<int[,]> spinData = new();
        public int[,] currentRotation { get; private set; }
        public int[,] requiredCorners { get; private set; }
        public int rotationId { get; protected set; } = 0;
        public static int offset { get; set; }

        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public int initOffsetX { get; set; }
        public int initOffsetY { get; set; }
        public Color color { get; protected set; } = Color.White;
        public TetrominoType type { get; private set; }
        public int[,] thumbnail;

        public Piece() { }

        public Piece(TetrominoType type, List<int[,]> rotations, List<int[,]> spinData, Color color, int[,] thumbnail)
        {
            this.type = type;
            this.rotations = rotations;
            this.spinData = spinData;
            this.color = color;
            this.thumbnail = thumbnail;
            currentRotation = rotations[rotationId];
            requiredCorners = spinData[rotationId];
        }


        public void ResetId()
        {
            rotationId = 0;
        }

        public Vector2 GetPixelCenterOfRotation()
        {
            List<int> xlen = [];
            List<int> ylen = [];
            
            for (var x = 0; x < currentRotation.GetLength(1); x++) // horizontal
            {
                for (var y = 0; y < currentRotation.GetLength(0); y++) // vertical
                {
                    if (currentRotation[y, x] > 0)
                    {
                        xlen.Add(currentRotation[y, x]);
                        break;
                    }
                }
            }

            for (var y = 0; y < currentRotation.GetLength(0); y++) // vertical
            {
                for (var x = 0; x < currentRotation.GetLength(1); x++) // horizontal
                {
                    if (currentRotation[y, x] > 0)
                    {
                        ylen.Add(currentRotation[y, x]);
                        break;
                    }

                    
                }
            }
            return new Vector2((xlen.Count * 8) / 2, (ylen.Count * 8) / 2);
        }

        public List<int> GetNonEmptyColumns() 
        {
            int count = 0;
            List<int> columns = [];
            for (var x = 0; x < currentRotation.GetLength(1); x++) 
            {
                for (var y = 0; y < currentRotation.GetLength(1); y++) 
                {
                    if (currentRotation[y, x] > 0) 
                    {
                        count++;
                        columns.Add(x);
                        break;
                    }
                }
            }
            return columns;
        }

        public int GetEmptyColumns() 
        {
            int count = 0;
            for (var x = 0; x < currentRotation.GetLength(1); x++)
            {
                var isEmpty = true;
                for (var y = 0; y < currentRotation.GetLength(1); y++)
                {
                    if (currentRotation[y, x] > 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty) count++;
                else break;
            }
            return count;
        }

        public int GetNonEmptyRows() 
        {
            int count = 0;

            for (var y = 0; y < currentRotation.GetLength(0); y++) 
            {
                for (var x = 0; x < currentRotation.GetLength(0); x++) 
                {
                    if (currentRotation[y, x] > 0) 
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        public int GetEmptyRows() 
        {
            int count = 0;
            for(var y = 0; y < currentRotation.GetLength(0); y++) 
            {
                var isEmpty = true;
                for (var x = 0; x < currentRotation.GetLength(1); x++) 
                {
                    if (currentRotation[y, x] > 0) 
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty) count++;
                else break;
            }
            return count;
        }

        public Point GetCenterPtOffset() 
        {
            int Xoff = 0;
            int Yoff = 0;
            for (var y = 0; y < requiredCorners.GetLength(0); y++) 
            {
                
                for (var x = 0; x < requiredCorners.GetLength(1); x++) 
                {
                    Xoff = x;
                    if (requiredCorners[y, x] == 3)
                        break;
                }
                Yoff = y;
                Xoff = 0;
            }
            return new Point(0, Yoff);
        }

        public int GetLowestPoint(int columnOffset) 
        {
            int Yoff = 0;
            for (var y = 0; y < requiredCorners.GetLength(0); y++) 
            {
                if (currentRotation[y, columnOffset] > 0) break;
                Yoff++;
            }
            return Yoff;
        }

        public (int row, int rowLength) GetLongestRow() // if this doesn't work im killing myself
        {
            List<int> rows = [];
            (int row, int rowLength) longestRow = (0, int.MinValue);
            for (var y = 0; y < currentRotation.GetLength(0); y++) 
            {
                var counter = 0;
                for (var x = 0; x < currentRotation.GetLength(1); x++) 
                {
                    if (currentRotation[y, x] > 0)
                        counter++;
                }
                if (counter > longestRow.rowLength)
                    longestRow = (y, counter);
            }
            return longestRow;
        }

        public void RotateCW() 
        {
            rotationId++;
            if (rotationId > rotations.Count - 1)
                rotationId = 0;
        }

        public void RotateCCW() 
        {
            rotationId--;
            if (rotationId < 0)
                rotationId = rotations.Count - 1;
        }

        public int ProjectRotateCW()
        {
            int projection = rotationId;
            projection++;
            if (projection > rotations.Count - 1)
                projection = 0;

            return projection;
        }

        public int ProjectRotateCCW()
        {
            int projection = rotationId;
            projection--;
            if (projection < 0)
                projection = rotations.Count - 1;

            return projection;
        }

        public void Update() 
        {
            currentRotation = rotations[rotationId];
            requiredCorners = spinData[rotationId];
        }
    }
}
