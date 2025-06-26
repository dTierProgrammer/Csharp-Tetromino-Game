using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface.Input
{
    public class InputManager
    {
        Keys shiftPieceLeft;
        Keys shiftPieceRight;
        Keys specialDrop;
        Keys softDrop;
        Keys rotateCw;
        Keys rotateCwAlt;
        Keys rotateCcw;
        Keys rotateCcwAlt;
        Keys rotate180;
        Keys hold;

        Keys menuMoveUp;
        Keys menuMoveDown;
        Keys menuMoveLeft;
        Keys menuMoveRight;
        Keys menuConfirm;
        Keys menuDeny;
        Keys menuTabLeft;
        Keys menyTabRight;

        Keys pause;

        Buttons shiftPieceLeftPad;
        Buttons shiftPieceRightPad;
        Buttons specialDropPad;
        Buttons softDropPad;
        Buttons rotateCwPad;
        Buttons rotateCwAltPad;
        Buttons rotateCcwPad;
        Buttons rotateCcwAltPad;
        Buttons rotate180Pad;
        Buttons holdPad;

        Buttons menuMoveUpPad;
        Buttons menuMoveDownPad;
        Buttons menuMoveLeftPad;
        Buttons menuMoveRightPad;
        Buttons menuConfirmPad;
        Buttons menuDenyPad;
        Buttons menuTabLeftPad;
        Buttons menuTabRightPad;

        Buttons pausePad;

        Queue<KeyEvent> keyEventQueue = new();
        float bufferWindow = .2f;

        public void GetInput(float deltaTime) 
        {
            if (Keyboard.GetState().IsKeyDown(shiftPieceLeft)) 
                keyEventQueue.Enqueue(new KeyEvent(shiftPieceLeft, deltaTime));
            if (Keyboard.GetState().IsKeyDown(shiftPieceRight)) 
                keyEventQueue.Enqueue(new KeyEvent(shiftPieceRight, deltaTime));
            if(Keyboard.GetState().IsKeyDown(rotateCw))
                keyEventQueue.Enqueue(new KeyEvent(rotateCw, deltaTime));
            if (Keyboard.GetState().IsKeyDown(rotateCcw))
                keyEventQueue.Enqueue(new KeyEvent(rotateCcw, deltaTime));
        }

        public KeyEvent? GetBufferedInput(float deltaTime) 
        {
            foreach (var item in keyEventQueue) 
            {
                if (deltaTime - item.timePressed <= bufferWindow) 
                {
                    return keyEventQueue.Dequeue();
                }
            }
            return null;
        }
    }
}
