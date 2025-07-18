using System;
using System.Collections.Generic;
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

        public float offsetX { get; set; }
        public float offsetY { get; set; }
        public float initOffsetX { get; set; }
        public float initOffsetY { get; set; }
        public Color color { get; protected set; } = Color.White;
        public TetrominoType type { get; private set; }

        public Piece() { }

        public Piece(TetrominoType type, List<int[,]> rotations, List<int[,]> spinData, Color color) 
        { 
            this.type = type;
            this.rotations = rotations;
            this.spinData = spinData;
            this.color = color;
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
