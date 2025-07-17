using System.Collections;
using System.Collections.Generic;

namespace MonoStacker.Source.GameObj.Tetromino.RandGenerator;

public class TIRandomizer
{ // tgm3 randomizer
    /*
    35pc bag, 5 of each piece
    every draw, get oldest piece and place into bag
    */
    private readonly Queue<TetrominoType> _pieceQueue;

}