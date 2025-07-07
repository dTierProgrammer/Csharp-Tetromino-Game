using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Generic
{
    public abstract class Piece
    {
        public readonly List<int[,]> rotations = new();
        public readonly List<int[,]> spinData = new();
        public int[,] currentRotation { get; protected set; }
        public int[,] requiredCorners { get; protected set; }
        public int rotationId { get; protected set; } = 0;
        public static int offset { get; set; }

        public float offsetX { get; set; }
        public float offsetY { get; set; }
        public float initOffsetX { get; set; }
        public float initOffsetY { get; set; }
        public Color color { get; protected set; }


        public void ResetId()
        {
            rotationId = 0;
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
