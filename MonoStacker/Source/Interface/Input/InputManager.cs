using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.Interface.Input
{
    public enum GameAction 
    {
        None = 0,
        MovePieceLeft = 1,
        MovePieceRight = 2,
        RotateCw = 3,
        RotateCcw = 4,
        Rotate180 = 5,
        HardDrop = 6,
        FirmDrop = 7,
        SoftDrop = 8,
        Hold = 9,
        RotateCwAlt = 10,
        RotateCcwAlt = 11
    }

    public class InputManager
    {
        private readonly Keys _keyMovePieceLeft = Keys.Left;
        private readonly Keys _keyMovePieceRight = Keys.Right;
        private readonly Keys _keyRotateCw = Keys.Up;
        private readonly Keys _keyRotateCcw = Keys.Z;
        private readonly Keys _keyRotate180;
        private readonly Keys _keyHardDrop = Keys.Space;
        private readonly Keys _keyFirmDrop;
        private readonly Keys _keySoftDrop = Keys.Down;
        private readonly Keys _keyHold = Keys.LeftShift;
        private readonly Keys _keyRotateCwAlt;
        public readonly Keys _keyRotateCcwAlt;
        private KeyboardState _priorKbState;
        
        private readonly Buttons _btnMovePieceLeft = Buttons.DPadLeft;
        private readonly Buttons _btnMovePieceRight = Buttons.DPadRight;
        private readonly Buttons _btnRotateCw = Buttons.A;
        private readonly Buttons _btnRotateCcw = Buttons.B;
        private readonly Buttons _btnRotate180;
        private readonly Buttons _btnHardDrop = Buttons.RightTrigger;
        private readonly Buttons _btnFirmDrop;
        private readonly Buttons _btnSoftDrop = Buttons.DPadDown;
        private readonly Buttons _btnHold = Buttons.LeftTrigger;
        private readonly Buttons _btnRotateCwAlt;
        private readonly Buttons _btnRotateCcwAlt;
        private GamePadState _priorPadState;

        public Queue<InputEvent> bufferQueue { get; private set; } = [];
        
        public int bufferCapacity { get; set; } = 10;

        public void ClearBuffer()
        {
            bufferQueue.Clear();
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
            List < InputEvent > returnValue = [];
            foreach (var item in bufferQueue)
                returnValue.Add(item);
            //returnValue.Sort();
            foreach (var item in returnValue)
            {
                if(item.gameAction is GameAction.Hold || item.gameAction is GameAction.RotateCcw)
                    Console.WriteLine(item.gameAction);
            }
            return returnValue;
        }

        public InputEvent GetFirstBufferedAction() 
        {
            return bufferQueue.Dequeue();
        }

       

        public List<GameAction> GetKeyInput() 
        {
            List<GameAction> currentKeys = new();
            if (Keyboard.GetState().IsKeyDown(_keyHold))
                currentKeys.Add(GameAction.Hold);
            if (Keyboard.GetState().IsKeyDown(_keyRotateCw))
                currentKeys.Add(GameAction.RotateCw);
            if (Keyboard.GetState().IsKeyDown(_keyRotateCcw))
                currentKeys.Add(GameAction.RotateCcw);
            if (Keyboard.GetState().IsKeyDown(_keyHardDrop))
                currentKeys.Add(GameAction.HardDrop);
            if (Keyboard.GetState().IsKeyDown(_keySoftDrop))
                currentKeys.Add(GameAction.SoftDrop);
            if (Keyboard.GetState().IsKeyDown(_keyMovePieceRight))
                currentKeys.Add(GameAction.MovePieceRight);
            if (Keyboard.GetState().IsKeyDown(_keyMovePieceLeft))
                currentKeys.Add(GameAction.MovePieceLeft);
            if(Keyboard.GetState().IsKeyDown(_keySoftDrop))
                currentKeys.Add(GameAction.SoftDrop);

            return currentKeys;
        }

        public void BufferKeyInput(GameTime gameTime) 
        {
            Keys[] keys =
            [
                _keyHold,
                _keyRotateCw,
                _keyRotateCcw,
                _keyRotate180,
                _keyRotateCwAlt,
                _keyRotateCcwAlt
            ];

            foreach (var key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key) && !_priorKbState.IsKeyDown(key)) 
                    AddToBuffer(new InputEvent { gameAction = ConvKeyToAction(key), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorKbState = Keyboard.GetState();
        }

        private GameAction ConvKeyToAction(Keys item) 
        {
            if (item == _keyMovePieceLeft) return GameAction.MovePieceLeft;
            if (item == _keyMovePieceRight) return GameAction.MovePieceRight;
            if (item == _keyRotateCw) return GameAction.RotateCw;
            if (item == _keyRotateCcw) return GameAction.RotateCcw;
            if (item == _keyRotate180) return GameAction.Rotate180;
            if (item == _keyHardDrop) return GameAction.HardDrop;
            if (item == _keyFirmDrop) return GameAction.FirmDrop;
            if (item == _keySoftDrop) return GameAction.SoftDrop;
            if (item == _keyHold) return GameAction.Hold;
            if (item == _keyRotateCwAlt) return GameAction.RotateCwAlt;
            if (item == _keyRotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
        
        public List<GameAction> GetButtonInput()
        {
            List<GameAction> currentButtons = new();
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnRotateCw))
                currentButtons.Add(GameAction.RotateCw);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnRotateCcw))
                currentButtons.Add(GameAction.RotateCcw);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnHardDrop))
                currentButtons.Add(GameAction.HardDrop);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnSoftDrop))
                currentButtons.Add(GameAction.SoftDrop);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnHold))
                currentButtons.Add(GameAction.Hold);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnMovePieceRight))
                currentButtons.Add(GameAction.MovePieceRight);
            if(GamePad.GetState(PlayerIndex.One).IsButtonDown(_btnMovePieceLeft))
                currentButtons.Add(GameAction.MovePieceLeft);
            
            return currentButtons;
        }
        
        public void BufferButtonInput(GameTime gameTime) 
        {
            Buttons[] buttons =
            [
                _btnMovePieceLeft,
                _btnMovePieceLeft,
                _btnRotateCw,
                _btnRotateCcw,
                _btnRotate180,
                _btnHold,
                _btnHardDrop,
                _btnSoftDrop,
                _btnRotateCwAlt,
                _btnRotateCcwAlt
            ];

            foreach (var button in buttons)
            {
                if (GamePad.GetState(0).IsButtonDown(button) && !_priorPadState.IsButtonDown(button)) 
                    AddToBuffer(new InputEvent { gameAction = ConvButtonToAction(button), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorPadState = GamePad.GetState(0);
        }
        
        private GameAction ConvButtonToAction(Buttons item) 
        {
            if (item == _btnMovePieceLeft) return GameAction.MovePieceLeft;
            if (item == _btnMovePieceRight) return GameAction.MovePieceRight;
            if (item == _btnRotateCw) return GameAction.RotateCw;
            if (item == _btnRotateCcw) return GameAction.RotateCcw;
            if (item == _btnRotate180) return GameAction.Rotate180;
            if (item == _btnHardDrop) return GameAction.HardDrop;
            if (item == _btnFirmDrop) return GameAction.FirmDrop;
            if (item == _btnSoftDrop) return GameAction.SoftDrop;
            if (item == _btnHold) return GameAction.Hold;
            if (item == _btnRotateCwAlt) return GameAction.RotateCwAlt;
            if (item == _btnRotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
    }
}
