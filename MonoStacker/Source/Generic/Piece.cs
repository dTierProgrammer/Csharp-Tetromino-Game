using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic
{
    public abstract class Piece
    {
        public readonly List<int[,]> rotations = new();
        public int[,] currentRotation { get; protected set; }
        public int rotationId { get; set; }
        public Piece() { rotationId = 0;  }
        public static int offset { get; set; }

        public float offsetX { get; set; }
        public float offsetY { get; set; }
        public float initOffsetX { get; set; }
        public float initOffsetY { get; set; }

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

        public void Update() 
        {
            currentRotation = rotations[rotationId];
        }
    }
}
