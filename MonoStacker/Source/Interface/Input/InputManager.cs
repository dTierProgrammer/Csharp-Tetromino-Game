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



        public Buttons b_MovePieceLeft { get; set; } = Buttons.DPadLeft;
        public Buttons b_MovePieceRight { get; set; } = Buttons.DPadRight;
        public Buttons b_RotateCw { get; set; } = Buttons.A;
        public Buttons b_RotateCcw { get; set; } = Buttons.B;
        public Buttons b_Rotate180 { get; set; }
        public Buttons b_HardDrop { get; set; } = Buttons.DPadUp;
        public Buttons b_FirmDrop { get; set; }
        public Buttons b_SoftDrop { get; set; } = Buttons.DPadDown;
        public Buttons b_Hold { get; set; } = Buttons.LeftTrigger;
        
        public Buttons b_RotateCwAlt { get; set; }
        public Buttons b_RotateCcwAlt { get; set; }

        private GamePadState _priorPadState;

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
            if (Keyboard.GetState().IsKeyDown(k_SoftDrop))
                currentKeys.Add(GameAction.SoftDrop);
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
                k_SoftDrop,
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
        
        public List<GameAction> GetButtonInput()
        {
            List<GameAction> currentButtons = new();
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_RotateCw))
                currentButtons.Add(GameAction.RotateCw);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_RotateCcw))
                currentButtons.Add(GameAction.RotateCcw);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_HardDrop))
                currentButtons.Add(GameAction.HardDrop);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_SoftDrop))
                currentButtons.Add(GameAction.SoftDrop);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_Hold))
                currentButtons.Add(GameAction.Hold);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_MovePieceRight))
                currentButtons.Add(GameAction.MovePieceRight);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(b_MovePieceLeft))
                currentButtons.Add(GameAction.MovePieceLeft);
            
            return currentButtons;
        }
        
        public void BufferButtonInput(GameTime gameTime) 
        {
            Buttons[] buttons = new Buttons[]
            {
                b_MovePieceLeft,
                b_MovePieceLeft,
                b_RotateCw,
                b_RotateCcw,
                b_Rotate180,
                b_Hold,
                b_HardDrop,
                b_SoftDrop,
                b_RotateCwAlt,
                b_RotateCcwAlt
            };

            foreach (var button in buttons)
            {
                if (GamePad.GetState(0).IsButtonDown(button) && !_priorPadState.IsButtonDown(button)) 
                    AddToBuffer(new InputEvent { gameAction = ConvButtonToAction(button), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorPadState = GamePad.GetState(0);
        }
        
        protected GameAction ConvButtonToAction(Buttons item) 
        {
            if (item == b_MovePieceLeft) return GameAction.MovePieceLeft;
            if (item == b_MovePieceRight) return GameAction.MovePieceRight;
            if (item == b_RotateCw) return GameAction.RotateCw;
            if (item == b_RotateCcw) return GameAction.RotateCcw;
            if (item == b_Rotate180) return GameAction.Rotate180;
            if (item == b_HardDrop) return GameAction.HardDrop;
            if (item == b_FirmDrop) return GameAction.FirmDrop;
            if (item == b_SoftDrop) return GameAction.SoftDrop;
            if (item == b_Hold) return GameAction.Hold;
            if (item == b_RotateCwAlt) return GameAction.RotateCwAlt;
            if (item == b_RotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
    }
}
