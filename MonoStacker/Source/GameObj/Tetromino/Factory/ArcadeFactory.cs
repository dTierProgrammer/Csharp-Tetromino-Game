using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoStacker.Source.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.Factory;

public class ArcadeFactory: ITetrominoFactory
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
                    { 7, 7, 7, 7 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 }
                });

                rotations.Add(new [,]
                {
                    { 0, 0, 7, 0 },
                    { 0, 0, 7, 0 },
                    { 0, 0, 7, 0 },
                    { 0, 0, 7, 0 }
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

                break;
            case TetrominoType.J:
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
                
                rotations.Add(new [,] 
                {
                    { 0, 0, 0},
                    { 2, 0, 0},
                    { 2, 2, 2}
                });

                rotations.Add(new [,]
                {
                    { 0, 2, 2},
                    { 0, 2, 0},
                    { 0, 2, 0}
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

                break;
            case TetrominoType.L:
                

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
                
                rotations.Add(new[,]
                {
                    { 0, 0, 0},
                    { 0, 0, 3},
                    { 3, 3, 3}
                });

                rotations.Add(new [,]
                {
                    { 0, 3, 0},
                    { 0, 3, 0},
                    { 0, 3, 3}
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
                    { 0, 0, 0},
                    { 0, 6, 6},
                    { 6, 6, 0}
                });
                rotations.Add(new [,]
                {
                    { 6, 0, 0},
                    { 6, 6, 0},
                    { 0, 6, 0}
                });
                spinData.Add(new [,]
                {
                    { 2, 0, 0},
                    { 0, 3, 2},
                    { 0, 0, 0}
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
                    { 0, 0, 0},
                    { 1, 1, 1},
                    { 0, 1, 0}
                });
                
                rotations.Add(new [,]
                {
                    { 0, 1, 0},
                    { 1, 1, 0},
                    { 0, 1, 0}
                });
                
                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 1, 0},
                    { 1, 1, 1}
                });

                rotations.Add(new [,]
                {
                    { 0, 1, 0},
                    { 0, 1, 1},
                    { 0, 1, 0}
                });
                spinData.Add(new [,]
                {
                    {0, 0, 0},
                    {0, 0, 0},
                    {0, 3, 2}
                });
                spinData.Add(new [,]
                {
                    {0, 0, 0},
                    {0, 3, 0},
                    {0, 0, 2}
                });
                spinData.Add(new [,]
                {
                    {0, 0, 0},
                    {0, 3, 0},
                    {0, 0, 2}
                });
                spinData.Add(new [,]
                {
                    {0, 0, 0},
                    {0, 3, 0},
                    {0, 0, 2}
                });

                break;
            case TetrominoType.Z:
                rotations.Add(new [,]
                {
                    { 0, 0, 0},
                    { 5, 5, 0},
                    { 0, 5, 5}
                });
                rotations.Add(new [,]
                {
                    { 0, 0, 5},
                    { 0, 5, 5},
                    { 0, 5, 0}
                });
                spinData.Add(new [,]
                {
                    { 0, 0, 0},
                    { 0, 3, 2},
                    { 2, 0, 0}
                });
                spinData.Add(new [,]
                {
                    { 0, 2, 0},
                    { 0, 3, 0},
                    { 0, 0, 2}
                });

                break;
        }
        Color color = type switch
        {
            TetrominoType.I => Color.Red,
            TetrominoType.J => Color.RoyalBlue,
            TetrominoType.L => Color.Orange,
            TetrominoType.O => Color.Yellow,
            TetrominoType.S => Color.Magenta,
            TetrominoType.T => Color.Cyan,
            TetrominoType.Z => new Color(0, 255, 0),
            _ => Color.White
        };
        
        Piece piece = new();

        var thumbnail = type switch 
        {
            TetrominoType.I => new int[,]
                {
                    { 7, 7, 7, 7 },
                    { 0, 0, 0, 0 }
                },
            TetrominoType.J => new int[,]
                {
                    { 2, 2, 2 },
                    { 0, 0, 2 }
                },
            TetrominoType.L => new int[,]
                {
                    { 3, 3, 3 },
                    { 3, 0, 0 }
                },
            TetrominoType.O => new int[,]
                {
                    { 0, 4, 4, 0 },
                    { 0, 4, 4, 0 }
                },
            TetrominoType.S => new int[,]
                {
                    { 0, 6, 6 },
                    { 6, 6, 0 }
                },
            TetrominoType.T => new int[,]
                {
                    { 1, 1, 1 },
                    { 0, 1, 0 }
                },
            TetrominoType.Z => new int[,]
                {
                    { 5, 5, 0 },
                    { 0, 5, 5 }
                },
            _ =>  new int[,] { { 1 } }
        };
        
        return new Piece(type, rotations, spinData, color, thumbnail);
    }

    public int[][] SpawnArea()
    {
        var area = new int[2][];
        for (int i = 0; i < 2; i++)
        {
            var length = i switch
            {
                1 => 3,
                _ => 4
            };

            area[i] = new int[length];
        }

        return area;
    }

    public Point SpawnOffset_Jlstz()
    {
        return new Point(0, -1);
    }

    public Point SpawnOffset_O()
    {
        return new Point(1, 0);
    }

    public Point SpawnOffset_I()
    {
        return new Point(0, -1);
    }
}