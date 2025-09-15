using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace MonoStacker.Source.Generic
{
    public static class LineFactory
    {
        public static int[] CreateLine(int hole, int holeLength, int colorId) 
        {
            int[] line = new int[Grid.COLUMNS];
            for (var i = 0; i < line.GetLength(0); i++) 
            {
                line[i] = colorId;
            }

            for (var i = 0; i < line.GetLength(0); i++) 
            {
                if (i == hole) 
                {
                    line[i] = 0;
                    for (var x = 1; x <= holeLength; x++) 
                        line[i + x] = 0;
                }
            }

            return line;
        }

        public static int[] CopyLineFromGrid(int id, Grid grid) 
        {
            int[] line = new int[Grid.COLUMNS];

            for (var i = 0; i < line.GetLength(0); i++) 
                line[i] = grid._matrix[id][i];

            return line;
        }

        public static int[] CopyLineFrom2dArray(int id, int[,] data)
        {
            int[] line = new int[data.GetLength(1)];

            for (var i = 0; i < line.GetLength(0); i++)
                line[i] = data[id, i];

            return line;
        }

        public static int[] CopyLineFromJaggedArray(int id, int[][] data)
        {
            int[] line = new int[data[id].GetLength(0)];

            for (var i = 0; i < line.GetLength(0); i++)
                line[i] = data[id][i];

            return line;
        }
    }
}
