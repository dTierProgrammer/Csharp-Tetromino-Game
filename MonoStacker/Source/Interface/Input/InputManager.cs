using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Interface.Input
{
    public enum GameAction 
    {
        None,
        MovePieceLeft,
        MovePieceRight,
        RotateCw,
        RotateCcw,
        Rotate180,
        HardDrop,
        FirmDrop,
        SoftDrop,
        Hold,
        RotateCwAlt,
        RotateCcwAlt
    }

    public class InputManager
    {
        public Keys k_MovePieceLeft { get; set; } = Keys.Left;
        public Keys k_MovePieceRight { get; set; } = Keys.Right;
        public Keys k_RotateCw { get; set; } = Keys.Up;
        public Keys k_RotateCcw { get; set; } = Keys.Z;
        public Keys k_Rotate180 { get; set; }
        public Keys k_HardDrop { get; set; } = Keys.Space;
        public Keys k_FirmDrop { get; set; }
        public Keys k_SoftDrop { get; set; } = Keys.Down;
        public Keys k_Hold { get; set; } = Keys.LeftShift;

        public Keys k_RotateCwAlt { get; set; }
        public Keys k_RotateCcwAlt { get; set; }

        private KeyboardState _priorKbState;

        public Queue<InputEvent> bufferQueue { get; private set; } = new();
        public Queue<InputEvent> holdBufferQueue { get; private set; } = new();
        public int bufferCapacity { get; set; } = 6;

        public void ClearBuffer()
        {
            bufferQueue.Clear();
        }
        public void ClearHoldBuffer()
        {
            holdBufferQueue.Clear();
        }

        private bool CheckForAction(GameAction action)
        {
            foreach (var item in bufferQueue) 
            {
                if (item.gameAction == action)
                    return true;
            }
            return false;
        }

        private void AddToBuffer(InputEvent item) 
        {
            if (bufferQueue.Count() < bufferCapacity)
            {
                switch (bufferQueue.Count())
                {
                    case 0: bufferQueue.Enqueue(item); break;
                    default: if (!CheckForAction(item.gameAction)) { bufferQueue.Enqueue(item); } break;
                }
            }
            else
            {
                bufferQueue.Dequeue();
                bufferQueue.Enqueue(item);
            }
        }

        public List<InputEvent> GetBufferedActions() 
        {
            List < InputEvent > returnValue = new();
            foreach (var item in bufferQueue)
                returnValue.Add(item);

            return returnValue;
        }

        public InputEvent GetFirstBufferedAction() 
        {
            InputEvent item = bufferQueue.Dequeue();
            return item;
        }

       

        public List<GameAction> GetKeyInput() 
        {
            List<GameAction> currentKeys = new();
            if (Keyboard.GetState().IsKeyDown(k_RotateCw))
                currentKeys.Add(GameAction.RotateCw);
            if (Keyboard.GetState().IsKeyDown(k_RotateCcw))
                currentKeys.Add(GameAction.RotateCcw);
            if (Keyboard.GetState().IsKeyDown(k_HardDrop))
                currentKeys.Add(GameAction.HardDrop);
            if (Keyboard.GetState().IsKeyDown(k_Hold))
                currentKeys.Add(GameAction.Hold);
            if (Keyboard.GetState().IsKeyDown(k_MovePieceRight))
                currentKeys.Add(GameAction.MovePieceRight);
            if (Keyboard.GetState().IsKeyDown(k_MovePieceLeft))
                currentKeys.Add(GameAction.MovePieceLeft);
            if(Keyboard.GetState().IsKeyDown(k_SoftDrop))
                currentKeys.Add(GameAction.SoftDrop);

            return currentKeys;
        }

        public void BufferKeyInput(GameTime gameTime) 
        {
            Keys[] keys = new Keys[]
            {
                k_MovePieceLeft,
                k_MovePieceRight,
                k_RotateCw,
                k_RotateCcw,
                k_Rotate180,
                k_Hold,
                k_HardDrop,
                k_RotateCwAlt,
                k_RotateCcwAlt
            };

            foreach (var key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key) && !_priorKbState.IsKeyDown(key)) 
                    AddToBuffer(new InputEvent { gameAction = ConvKeyToAction(key), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorKbState = Keyboard.GetState();
        }

        protected GameAction ConvKeyToAction(Keys item) 
        {
            if (item == k_MovePieceLeft) return GameAction.MovePieceLeft;
            if (item == k_MovePieceRight) return GameAction.MovePieceRight;
            if (item == k_RotateCw) return GameAction.RotateCw;
            if (item == k_RotateCcw) return GameAction.RotateCcw;
            if (item == k_Rotate180) return GameAction.Rotate180;
            if (item == k_HardDrop) return GameAction.HardDrop;
            if (item == k_FirmDrop) return GameAction.FirmDrop;
            if (item == k_SoftDrop) return GameAction.SoftDrop;
            if (item == k_Hold) return GameAction.Hold;
            if (item == k_RotateCwAlt) return GameAction.RotateCwAlt;
            if (item == k_RotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
    }
}
