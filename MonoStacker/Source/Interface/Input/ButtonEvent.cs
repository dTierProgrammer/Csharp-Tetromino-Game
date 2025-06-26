using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface.Input
{
    public struct ButtonEvent
    {
        public Buttons buttonPressed { get; private set; }
        public float timePressed { get; private set; }
        public float timeHeld { get; private set; }

        public ButtonEvent(Buttons buttonPressed, float timePressed, float timeHeld)
        {
            this.buttonPressed = buttonPressed;
            this.timePressed = timePressed;
            this.timeHeld = timeHeld;
        }

        public ButtonEvent(Buttons buttonPressed, float timePressed)
        {
            this.buttonPressed = buttonPressed;
            this.timePressed = timePressed;
        }

        public ButtonEvent(Buttons buttonPressed)
        {
            this.buttonPressed = buttonPressed;
        }
    }
}
