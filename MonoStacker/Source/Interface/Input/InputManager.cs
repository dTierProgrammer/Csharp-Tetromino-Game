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
        private KeyboardState _priorKbState;
        private GamePadState _priorPadState;

        public readonly InputBinds _binds;

        public readonly Dictionary<GameAction, float> eventTimeStamps = [];

        public Queue<InputEvent> bufferQueue { get; private set; } = [];


        public InputManager(InputBinds binds) 
        {
            _binds = binds;
        }
        public int bufferCapacity { get; set; } = 10;

        public void ClearBuffer()
        {
            bufferQueue.Clear();
        }

        private bool CheckForAction(GameAction action) // checks for if an action is already in the buffer
        {
            foreach (var item in bufferQueue) 
            {
                if (item.gameAction == action)
                    return true;
            }
            return false;
        }

        private void AddToBuffer(InputEvent item) // enqueue an action to the buffer
        {
            if (bufferQueue.Count < bufferCapacity)
            {
                switch (bufferQueue.Count)
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

        public List<InputEvent> GetBufferedActions() // returns all currently buffered items as a list
        {
            List < InputEvent > returnValue = [];
            foreach (var item in bufferQueue)
                returnValue.Add(item);
            return returnValue;
        }

        public InputEvent GetFirstBufferedAction() // returns the first item in the buffer
        {
            return bufferQueue.Dequeue();
        }

        public void AddTimeStamp(GameTime gameTime, GameAction action) // add or update a timestamp for a specific action
        {
            if (eventTimeStamps.ContainsKey(action))
                eventTimeStamps[action] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                eventTimeStamps.TryAdd(action, (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void CleanOpposingTimestamps(GameAction item1, GameAction item2) // compare 2 opposing timestamps, clear the older one
        {
            if (eventTimeStamps.ContainsKey(item1) && eventTimeStamps.ContainsKey(item2))
            {
                if (eventTimeStamps[item1] > eventTimeStamps[item2])
                    eventTimeStamps.Remove(item2);
                else
                    eventTimeStamps.Remove(item1);
            }
        }

        public List<GameAction> GetKeyInput() // returns a list of game actions corresponding to the bound keys
        {
            List<GameAction> currentKeys = new();

            Keys[] keys =
            [
                _binds.k_Hold,
                _binds.k_RotateCw,
                _binds.k_RotateCcw,
                _binds.k_Rotate180,
                _binds.k_HardDrop,
                _binds.k_FirmDrop,
                _binds.k_SoftDrop,
                _binds.k_MovePieceRight,
                _binds.k_MovePieceLeft,
                _binds.k_RotateCwAlt,
                _binds.k_RotateCcwAlt
            ];

            foreach (var key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key))
                    currentKeys.Add(ConvKeyToAction(key));
            }

            return currentKeys;
        }

        public List<GameAction> GetKeyInputSelection(Array keys) // only return keys from a specific selection
        {
            List<GameAction> currentKeys = [];
            foreach (Keys key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key))
                    currentKeys.Add(ConvKeyToAction(key));
            }
            return currentKeys;
        }

        public void BufferKeyInput(GameTime gameTime) // enqueue game actions based on bound key presses
        {
            Keys[] keys =
            [
                _binds.k_Hold,
                _binds.k_RotateCw,
                _binds.k_RotateCcw,
                _binds.k_Rotate180,
                _binds.k_HardDrop,
                _binds.k_FirmDrop,
                _binds.k_SoftDrop,
                _binds.k_MovePieceRight,
                _binds.k_MovePieceLeft,
                _binds.k_RotateCwAlt,
                _binds.k_RotateCcwAlt
            ];

            foreach (var key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key) && !_priorKbState.IsKeyDown(key)) 
                    AddToBuffer(new InputEvent { gameAction = ConvKeyToAction(key), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorKbState = Keyboard.GetState();
        }

        public void BufferKeySelection(GameTime gameTime, Array keys) // only buffer keys from a specific selection
        {
            foreach (Keys key in keys)
            {
                if (Keyboard.GetState().IsKeyDown(key) && !_priorKbState.IsKeyDown(key)) 
                    AddToBuffer(new InputEvent { gameAction = ConvKeyToAction(key), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorKbState = Keyboard.GetState();
        }

        private GameAction ConvKeyToAction(Keys item) // convert a key to its corresponding game action
        {
            if (item == _binds.k_MovePieceLeft) return GameAction.MovePieceLeft;
            if (item == _binds.k_MovePieceRight) return GameAction.MovePieceRight;
            if (item == _binds.k_RotateCw) return GameAction.RotateCw;
            if (item == _binds.k_RotateCcw) return GameAction.RotateCcw;
            if (item == _binds.k_Rotate180) return GameAction.Rotate180;
            if (item == _binds.k_HardDrop) return GameAction.HardDrop;
            if (item == _binds.k_FirmDrop) return GameAction.FirmDrop;
            if (item == _binds.k_SoftDrop) return GameAction.SoftDrop;
            if (item == _binds.k_Hold) return GameAction.Hold;
            if (item == _binds.k_RotateCwAlt) return GameAction.RotateCwAlt;
            if (item == _binds.k_RotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
        
        public List<GameAction> GetButtonInput(PlayerIndex playerIndex) // returns a list of game actions corresponding to the bound buttons
        {
            List<GameAction> currentButtons = new();
            Buttons[] buttons =
            [
                _binds.b_Hold,
                _binds.b_RotateCw,
                _binds.b_RotateCcw,
                _binds.b_Rotate180,
                _binds.b_HardDrop,
                _binds.b_FirmDrop,
                _binds.b_SoftDrop,
                _binds.b_MovePieceRight,
                _binds.b_MovePieceLeft,
                _binds.b_RotateCwAlt,
                _binds.b_RotateCcwAlt
            ];
            foreach (var button in buttons)
            {
                if (GamePad.GetState(playerIndex).IsButtonDown(button))
                    currentButtons.Add(ConvButtonToAction(button));
            }

            return currentButtons;
        }

        public List<GameAction> GetButtonInputSelection(Array buttons, PlayerIndex playerIndex) // only return buttons from a specific selection
        {
            List<GameAction> currentButtons = [];
            foreach (Buttons button in buttons)
            {
                if (GamePad.GetState(playerIndex).IsButtonDown(button))
                    currentButtons.Add(ConvButtonToAction(button));
            }
            return currentButtons;
        }

        public void BufferButtonInput(GameTime gameTime, PlayerIndex playerIndex) // enqueue game actions based on bound button presses
        {
            Buttons[] buttons =
            [
                _binds.b_Hold,
                _binds.b_RotateCw,
                _binds.b_RotateCcw,
                _binds.b_Rotate180,
                _binds.b_HardDrop,
                _binds.b_FirmDrop,
                _binds.b_SoftDrop,
                _binds.b_MovePieceRight,
                _binds.b_MovePieceLeft,
                _binds.b_RotateCwAlt,
                _binds.b_RotateCcwAlt
            ];

            foreach (var button in buttons)
            {
                if (GamePad.GetState(playerIndex).IsButtonDown(button) && !_priorPadState.IsButtonDown(button)) 
                    AddToBuffer(new InputEvent { gameAction = ConvButtonToAction(button), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorPadState = GamePad.GetState(playerIndex);
        }

        public void BufferButtonSelection(GameTime gameTime, Array buttons, PlayerIndex playerIndex) // only buffer buttons from a specific selection 
        {
            foreach (Buttons button in buttons)
            {
                if (GamePad.GetState(playerIndex).IsButtonDown(button) && !_priorPadState.IsButtonDown(button)) 
                    AddToBuffer(new InputEvent { gameAction = ConvButtonToAction(button), timePressed = (float)gameTime.TotalGameTime.TotalSeconds });
            }
            _priorPadState = GamePad.GetState(playerIndex);
        }

        private GameAction ConvButtonToAction(Buttons item) // convert a button to its corresponding game action
        {
            if (item == _binds.b_MovePieceLeft) return GameAction.MovePieceLeft;
            if (item == _binds.b_MovePieceRight) return GameAction.MovePieceRight;
            if (item == _binds.b_RotateCw) return GameAction.RotateCw;
            if (item == _binds.b_RotateCcw) return GameAction.RotateCcw;
            if (item == _binds.b_Rotate180) return GameAction.Rotate180;
            if (item == _binds.b_HardDrop) return GameAction.HardDrop;
            if (item == _binds.b_FirmDrop) return GameAction.FirmDrop;
            if (item == _binds.b_SoftDrop) return GameAction.SoftDrop;
            if (item == _binds.b_Hold) return GameAction.Hold;
            if (item == _binds.b_RotateCwAlt) return GameAction.RotateCwAlt;
            if (item == _binds.b_RotateCcwAlt) return GameAction.RotateCcwAlt;
            return GameAction.None;
        }
    }
}
