using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface.Input
{
    public struct KeyEvent
    {
        public Keys keyPressed { get; private set; }
        public float timePressed { get; private set; }
        public float timeHeld { get; private set; }

        public KeyEvent(Keys keyPressed, float timePressed, float timeHeld) 
        {
            this.keyPressed = keyPressed;
            this.timePressed = timePressed;
            this.timeHeld = timeHeld;
        }

        public KeyEvent(Keys keyPressed, float timePressed) 
        {
            this.keyPressed = keyPressed;
            this.timePressed = timePressed;
        }

        public KeyEvent(Keys keyPressed) 
        {
            this.keyPressed = keyPressed;
        }
    }
}
