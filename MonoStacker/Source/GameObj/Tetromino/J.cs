using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    public class J: Piece
    {
        public J() 
        {
            rotations.Add(new int[,] 
            {
                { 2, 0, 0},
                { 2, 2, 2},
                { 0, 0, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 2, 2},
                { 0, 2, 0},
                { 0, 2, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 0},
                { 2, 2, 2},
                { 0, 0, 2}
            });

            rotations.Add(new int[,]
            {
                { 0, 2, 0},
                { 0, 2, 0},
                { 2, 2, 0}
            });

            currentRotation = rotations[rotationId];
        }
    }
}
