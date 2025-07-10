using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Factory;

public class SrsFactory: ITetrominoFactory
{
    public Piece NewPiece(TetrominoType type)
    {
        List <int[,]> rotations = [];
        List <int[,]> spinData = [];
        switch (type)
        {
            case TetrominoType.I:
                rotations.Add(new [,] 
                {
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 }
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 }
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 0 }
                });

                rotations.Add(new [,]
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 }
                });
                
                spinData.Add(new [,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 3, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 }
                });
                spinData.Add(new [,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 3, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 }
                });
                spinData.Add(new [,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 3, 0 },
                    { 0, 0, 0, 0 }
                });
                spinData.Add(new [,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 3, 0, 0 },
                    { 0, 0, 0, 0 }
                });
                break;
            case TetrominoType.J: 
                rotations.Add(new [,] 
                {
                    { 2, 0, 0},
                    { 2, 2, 2},
                    { 0, 0, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 2, 2},
                    { 0, 2, 0},
                    { 0, 2, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 2, 2, 2},
                    { 0, 0, 2}
                });

                rotations.Add(new [,]
                {
                    { 0, 2, 0},
                    { 0, 2, 0},
                    { 2, 2, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 2, 2},
                    { 0, 3, 0},
                    { 0, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 3, 2},
                    { 0, 0, 2}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 3, 0},
                    { 2, 2, 0}
                });

                spinData.Add(new [,]
                {
                    { 2, 0, 0},
                    { 2, 3, 0},
                    { 0, 0, 0}
                });
                break;
            case TetrominoType.L: 
                rotations.Add(new[,]
                {
                    { 0, 0, 3},
                    { 3, 3, 3},
                    { 0, 0, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 3, 0},
                    { 0, 3, 0},
                    { 0, 3, 3}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 3, 3, 3},
                    { 3, 0, 0}
                });

                rotations.Add(new [,]
                {
                    { 3, 3, 0},
                    { 0, 3, 0},
                    { 0, 3, 0}
                });

                spinData.Add(new [,]
                {
                    { 2, 2, 0},
                    { 0, 3, 0},
                    { 0, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 2},
                    { 0, 3, 2},
                    { 0, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 3, 0},
                    { 0, 2, 2}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 2, 3, 0},
                    { 2, 0, 0}
                });
                break;
            case TetrominoType.O: 
                rotations.Add(new [,] 
                {
                    { 4, 4},
                    { 4, 4}
                });

                spinData.Add(new [,]
                {
                    { 0, 3},
                    { 0, 0}
                });
                break;
            case TetrominoType.S: 
                rotations.Add(new [,] 
                {
                    { 0, 5, 5},
                    { 5, 5, 0},
                    { 0, 0, 0}
                });

                rotations.Add(new [,] 
                {
                    { 0, 5, 0},
                    { 0, 5, 5},
                    { 0, 0, 5}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 5, 5},
                    { 5, 5, 0}
                });

                rotations.Add(new [,]
                {
                    { 5, 0, 0},
                    { 5, 5, 0},
                    { 0, 5, 0}
                });

                spinData.Add(new [,]
                {
                    { 2, 0, 0},
                    { 0, 3, 2},
                    { 0, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 2},
                    { 0, 3, 0},
                    { 0, 2, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 2, 3, 0},
                    { 0, 0, 2}
                });

                spinData.Add(new [,]
                {
                    { 0, 2, 0},
                    { 0, 3, 0},
                    { 2, 0, 0}
                });
                break;
            case TetrominoType.T: 
                rotations.Add(new [,]
                {
                    { 0, 6, 0},
                    { 6, 6, 6},
                    { 0, 0, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 6, 0},
                    { 0, 6, 6},
                    { 0, 6, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 6, 6, 6},
                    { 0, 6, 0}
                });


                rotations.Add(new [,]
                {
                    { 0, 6, 0},
                    { 6, 6, 0},
                    { 0, 6, 0}
                });

                spinData.Add(new [,]
                {
                    {1, 0, 1},
                    {0, 3, 0},
                    {2, 0, 2}
                });

                spinData.Add(new [,]
                {
                    {2, 0, 1},
                    {0, 3, 0},
                    {2, 0, 1}
                });

                spinData.Add(new [,]
                {
                    {2, 0, 2},
                    {0, 3, 0},
                    {1, 0, 1}
                });

                spinData.Add(new [,]
                {
                    {1, 0, 2},
                    {0, 3, 0},
                    {1, 0, 2}
                });
                break;
            case TetrominoType.Z: 
                rotations.Add(new [,]
                {
                    { 7, 7, 0},
                    { 0, 7, 7},
                    { 0, 0, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 7},
                    { 0, 7, 7},
                    { 0, 7, 0}
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 7, 7, 0},
                    { 0, 7, 7}
                });
                
                rotations.Add(new [,]
                {
                    { 0, 7, 0},
                    { 7, 7, 0},
                    { 7, 0, 0}
                });

                spinData.Add(new [,] 
                {
                    { 0, 0, 2},
                    { 2, 3, 0},
                    { 0, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 0, 2, 0},
                    { 0, 3, 0},
                    { 0, 0, 2}
                });

                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 3, 2},
                    { 2, 0, 0}
                });

                spinData.Add(new [,]
                {
                    { 2, 0, 0},
                    { 0, 3, 0},
                    { 0, 2, 0}
                });
                break;
        }

        Color color = type switch
        {
            TetrominoType.I => Color.Cyan,
            TetrominoType.J => Color.Orange,
            TetrominoType.L => Color.RoyalBlue,
            TetrominoType.O => Color.Yellow,
            TetrominoType.S => new Color(0, 255, 0),
            TetrominoType.T => Color.Magenta,
            TetrominoType.Z => Color.Red,
            _ => Color.White
        };
        Piece piece = new();
        
        return new Piece(type, rotations, spinData, color);
    }
}