using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Data
{
    public struct SRSData
    {
        /*
        0 - Spawn / Up
        1/R - Right
        2 - Down
        3/L - Left
        */

        public static readonly Point[,] DataJlstz =
        {
            /* 0 -> 1 (0) */ { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) },
            /* 1 -> 0 (1) */ { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
            /* 1 -> 2 (2) */ { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
            /* 2 -> 1 (3) */ { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) },
            /* 2 -> 3 (4) */ { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) },
            /* 3 -> 2 (5) */ { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) },
            /* 3 -> 0 (6) */ { new(0, 0), new(-1, 0), new(-1,-1), new( 0, 2), new(-1, 2) },
            /* 0 -> 3 (7) */ { new(0, 0), new(1, 0), new(1, 1), new (0, -2), new(1, -2) }
        };

        public static readonly Point[,] DataJlstzAlt = // trying to add tgm styled kicks to SRS
        {
            /* 0 -> 1 (0) */ { new(0, 0), new(-1, 0), new (0, -1), new(-1, 1), new(0, -2), new(-1, -2) },
            /* 1 -> 0 (1) */ { new(0, 0), new(1, 0), new(0, -1), new(1, -1), new(0, 2), new(1, 2) },
            /* 1 -> 2 (2) */ { new(0, 0), new(1, 0), new (0, -1), new(1, -1), new(0, 2), new(1, 2) },
            /* 2 -> 1 (3) */ { new(0, 0), new(-1, 0), new(0, -1), new(-1, 1), new(0, -2), new(-1, -2) },
            /* 2 -> 3 (4) */ { new(0, 0), new(1, 0), new(0, -1),  new(1, 1), new(0, -2), new(1, -2) },
            /* 3 -> 2 (5) */ { new(0, 0), new(-1, 0), new(0, -1), new(-1, -1), new(0, 2), new(-1, 2) },
            /* 3 -> 0 (6) */ { new(0, 0), new(-1, 0), new(0, -1), new(-1,-1), new( 0, 2), new(-1, 2) },
            /* 0 -> 3 (7) */ { new(0, 0), new(1, 0), new (0, -1), new(1, 1), new (0, -2), new(1, -2) }
        };

        public static readonly Point[,] DataI = new Point[,]
        {
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2) },
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2) },
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1) },
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2, 1) },
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2) },
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2) },
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2,1) },
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1) } 
        };

        public static readonly Point[,] DataIArika = new Point[,]
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2, 1)}, // 2 -> R
            { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2)}, // 2 -> L
            { new(0, 0), new(-2, 0), new(1, 0), new(-2,-1), new(1, 2)}, // L -> 2
            { new(0, 0), new(1, 0), new(-2, 0), new(1,-2), new(-2,1)}, // L -> 0
            { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2,-1)}  // 0 -> L
        };

        public static Point[,] DataICW = new Point[,]
        {
            { new( 0, 0), new(-2, 0), new(1, 0), new(1, 2), new(-2,-1) },
            { new( 0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1,-2) },
        public static int? GetSrsChecks(int currentState, int projectedState) 
            { new( 0, 0), new(-2, 0), new(1, 0), new(-2, 1), new(1,-1) },
            switch (currentState, projectedState) 
            {
                case (0, 1): return 0;
                case (1, 0): return 1;
                case (1, 2): return 2;
                case (2, 1): return 3;
                case (2, 3): return 4;
                case (3, 2): return 5;
                case (3, 0): return 6;
                case (0, 3): return 7;
            }
            return null;
        }
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
