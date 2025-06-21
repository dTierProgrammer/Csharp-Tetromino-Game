using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    internal class T: Piece
    {
        public T() 
        {
            rotations.Add(new int[,]
            {
                { 0, 6, 0},
                { 6, 6, 6},
                { 0, 0, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 6, 0},
                { 0, 6, 6},
                { 0, 6, 0}
            });

            rotations.Add(new int[,]
            {
                { 0, 0, 0},
                { 6, 6, 6},
                { 0, 6, 0}
            });


            rotations.Add(new int[,]
            {
                { 0, 6, 0},
                { 6, 6, 0},
                { 0, 6, 0}
            });

            spinData.Add(new int[,]
            {
                {1, 0, 1},
                {0, 0, 0},
                {2, 0, 2}
            });

            spinData.Add(new int[,]
            {
                {2, 0, 1},
                {0, 0, 0},
                {2, 0, 1}
            });

            spinData.Add(new int[,]
            {
                {2, 0, 2},
                {0, 0, 0},
                {1, 0, 1}
            });

            spinData.Add(new int[,]
            {
                {1, 0, 2},
                {0, 0, 0},
                {1, 0, 2}
            });

            currentRotation = rotations[rotationId];
            requiredCorners = spinData[rotationId];
        }
    }
}
