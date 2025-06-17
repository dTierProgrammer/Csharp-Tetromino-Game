using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    internal class L: Piece
    {
        public L() 
        {
            rotations.Add(new int[,]
            {
                { 0, 0, 3},
                { 3, 3, 3},
                { 0, 0, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 3, 0},
                { 0, 3, 0},
                { 0, 3, 3}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 0},
                { 3, 3, 3},
                { 3, 0, 0}
            });

            rotations.Add(new int[,]
            {
                { 3, 3, 0},
                { 0, 3, 0},
                { 0, 3, 0}
            });

            currentRotation = rotations[rotationId];
        }
    }
}
