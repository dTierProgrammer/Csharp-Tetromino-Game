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
        public int rotationId { get; protected set; }
        public Piece() { rotationId = 0;  }
        public static int offset { get; set; }

        public int offsetX = 2;
        public int offsetY = 2;

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
