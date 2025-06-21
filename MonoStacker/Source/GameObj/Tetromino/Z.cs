using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    internal class Z: Piece
    {
        public Z() 
        {
            rotations.Add(new int[,]
            {
                { 7, 7, 0},
                { 0, 7, 7},
                { 0, 0, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 7},
                { 0, 7, 7},
                { 0, 7, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 0},
                { 7, 7, 0},
                { 0, 7, 7}
            });


            rotations.Add(new int[,]
            {
                { 0, 7, 0},
                { 7, 7, 0},
                { 7, 0, 0}
            });

            spinData.Add(new int[,] 
            {
                { 0, 0, 2},
                { 2, 0, 0},
                { 0, 0, 0}
            });

            spinData.Add(new int[,]
            {
                { 0, 2, 0},
                { 0, 0, 0},
                { 0, 0, 2}
            });

            spinData.Add(new int[,]
            {
                { 0, 0, 0},
                { 0, 0, 2},
                { 2, 0, 0}
            });

            spinData.Add(new int[,]
           {
                { 2, 0, 0},
                { 0, 0, 0},
                { 0, 2, 0}
           });

            currentRotation = rotations[rotationId];
            requiredCorners = spinData[rotationId];
        }
    }
}
