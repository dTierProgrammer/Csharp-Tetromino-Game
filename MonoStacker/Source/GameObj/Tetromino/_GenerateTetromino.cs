using MonoStacker.Source.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.GameObj.Tetromino
{
    public enum TetrominoType 
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z
    }

    public static class _GenerateTetromino
    {
        private static Random _rng = new Random();
        private static Array _tetrominos = Enum.GetValues(typeof(TetrominoType));
        private static List<TetrominoType> _tetrominoBag = new List<TetrominoType> 
        {
            TetrominoType.I,
            TetrominoType.O,
            TetrominoType.J,
            TetrominoType.L,
            TetrominoType.S,
            TetrominoType.T,
            TetrominoType.Z
        };

        public static Piece RandomTetromino() 
        {
            Piece tetromino = null;
            TetrominoType nextTetromino = (TetrominoType)_rng.Next(0, _tetrominos.Length);
            int yOff = 18;
            switch (nextTetromino) 
            {
                case TetrominoType.I:
                    tetromino = new I();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff - 1;
                    break;
                case TetrominoType.J:
                    tetromino = new J();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    break;
                case TetrominoType.L:
                    tetromino = new L();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    break;
                case TetrominoType.O:
                    tetromino = new O();
                    tetromino.offsetX = 4;
                    tetromino.offsetY = yOff;
                    break;
                case TetrominoType.S:
                    tetromino = new S();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    break;
                case TetrominoType.T:
                    tetromino = new T();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    break;
                case TetrominoType.Z:
                    tetromino = new Z();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    break;

            }
            return tetromino;
        }

        public static Piece RandomTetromino7Bag() 
        {
            Piece tetromino = null;
            if (_tetrominoBag.Count == 0) 
            {
                _tetrominoBag = new List<TetrominoType>
                {
                    TetrominoType.I,
                    TetrominoType.O,
                    TetrominoType.J,
                    TetrominoType.L,
                    TetrominoType.S,
                    TetrominoType.T,
                    TetrominoType.Z
                };
            }
            int nextTetromino = _rng.Next(_tetrominoBag.Count);
            int yOff = 18;
            switch (_tetrominoBag.ElementAt(nextTetromino)) 
            {
                case TetrominoType.I:
                    tetromino = new I();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff - 1;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.J:
                    tetromino = new J();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.L:
                    tetromino = new L();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.O:
                    tetromino = new O();
                    tetromino.offsetX = 4;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.S:
                    tetromino = new S();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = 3;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.T:
                    tetromino = new T();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
                case TetrominoType.Z:
                    tetromino = new Z();
                    tetromino.offsetX = 3;
                    tetromino.offsetY = yOff;
                    tetromino.initOffsetX = tetromino.offsetX;
                    tetromino.initOffsetY = tetromino.offsetY;
                    break;
            }

            _tetrominoBag.Remove(_tetrominoBag.ElementAt(nextTetromino));

            return tetromino;
        }
    }
}
