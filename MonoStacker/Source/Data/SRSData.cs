using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Data
{
    public static class SRSData
    {
        /*
        0 - Spawn / Up
        1/R - Right
        2 - Down
        3/L - Left
        
        0 -> R
            R -> 0

        R -> 2
            2 -> 

        2 -> L
            L -> 2

        L -> 0
            0 -> L
        */

        public static Point[,] DataJLSTZ = new Point[,] 
        {
            { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)}, // 0 -> R
            { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)}, // R -> 0
            { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)}, // R -> 2
            { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)}, // 2 -> R
            { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2)}, // 2 -> L
            { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2)}, // L -> 2
            { new(0, 0), new(-1, 0), new(-1,-1), new( 0, 2), new(-1, 2)}, // L -> 0
            { new(0, 0), new(1, 0), new(1, 1), new (0, -2), new(1, -2)}  // 0 -> L
        };

        public static Point[,] DataJLSTZCW = new Point[,] 
        {
            { new(-1, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)}, // 0 -> R
            { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)}, // R -> 2
            { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2)}, // 2 -> L
            { new(0, 0), new(-1, 0), new(-1,-1), new( 0, 2), new(-1, 2)} // L -> 0
        };

        public static Point[,] DataJLSTZCCW = new Point[,]
        {
            { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2)}, // R -> 0
            { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2)}, // 2 -> R
            { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2)}, // L -> 2
            { new(0, 0), new(1, 0), new(1, 1), new (0, -2), new(1, -2)}  // 0 -> L
        };

        public static Point[,] DataI = new Point[,]
        {
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2)}, // 0 -> R
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // R -> 0
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}, // R -> 2
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2, 1)}, // 2 -> R
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // 2 -> L
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2)}, // L -> 2
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2,1)}, // L -> 0
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}  // 0 -> L
        };

        public static Point[,] DataICW = new Point[,]
        {
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2)}, // 0 -> R
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}, // R -> 2
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // 2 -> L
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2,1)}, // L -> 0
        };

        public static Point[,] DataICCW = new Point[,] 
        {
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // R -> 0
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2, 1)}, // 2 -> R
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2)}, // L -> 2
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}  // 0 -> L
        };

        public static Point[,] DataIArika = new Point[,]
        {
            { new( 0, 0), new(-2, 0), new(1, 0), new(1, 2), new(-2,-1)}, // 0 -> R
            { new( 0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // R -> 0
            { new( 0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}, // R -> 2
            { new( 0, 0), new(-2, 0), new(1, 0), new(-2, 1), new(1,-1)}, // 2 -> R
            { new( 0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-1)}, // 2 -> L
            { new( 0, 0), new(1, 0), new(-2, 0), new(1, 2), new(-2,-1)}, // L -> 2
            { new( 0, 0), new(-2, 0), new(1, 0), new(-2, 1), new(1,-2)}, // L -> 0
            { new( 0, 0), new(2, 0), new(-1, 0), new(-1, 2), new(2,-1)}  // 0 -> L
        };
    }

}
