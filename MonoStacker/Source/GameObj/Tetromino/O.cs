using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino
{
    internal class O : Piece
    {
        public O()
        {
            rotations.Add(new int[,] 
            {
                { 4, 4},
                { 4, 4}
            });

            currentRotation = rotations[rotationId];
        }
    }
}
