using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    internal class S: Piece
    {
        public S() 
        {
            rotations.Add(new int[,] 
            {
                { 0, 5, 5},
                { 5, 5, 0},
                { 0, 0, 0}
            });

            rotations.Add(new int[,] 
            {
                { 0, 5, 0},
                { 0, 5, 5},
                { 0, 0, 5}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 0},
                { 0, 5, 5},
                { 5, 5, 0}
            });

            rotations.Add(new int[,]
            {
                { 5, 0, 0},
                { 5, 5, 0},
                { 0, 5, 0}
            });

            spinData.Add(new int[,]
            {
                { 2, 0, 0},
                { 0, 3, 2},
                { 0, 0, 0}
            });

            spinData.Add(new int[,]
            {
                { 0, 0, 2},
                { 0, 3, 0},
                { 0, 2, 0}
            });

            spinData.Add(new int[,]
            {
                { 0, 0, 0},
                { 2, 3, 0},
                { 0, 0, 2}
            });

            spinData.Add(new int[,]
           {
                { 0, 2, 0},
                { 0, 3, 0},
                { 2, 0, 0}
           });

            currentRotation = rotations[rotationId];
            requiredCorners = spinData[rotationId];
        }
    }
}
