namespace MonoStacker.Source.GameObj.Tetromino;

public enum TetrominoType 
{
    I = 0,
    J = 1,
    L = 2,
    O = 3,
    S = 4,
    T = 5,
    Z = 6
}

public static class ConvertTypeValue
{
    public static char GetTypeChar(TetrominoType type)
    {
        return type switch 
        {
            TetrominoType.I => 'I',
            TetrominoType.J => 'J',
            TetrominoType.L => 'L',
            TetrominoType.O => 'O',
            TetrominoType.S => 'S',
            TetrominoType.T => 'T',
            TetrominoType.Z => 'Z',
        };
    }
}
