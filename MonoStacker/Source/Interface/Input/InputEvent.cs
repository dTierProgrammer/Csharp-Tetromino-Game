using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface.Input
{
    public class InputEvent
    {
        public GameAction gameAction { get; set; }
        public float timePressed;
        public bool hasBeenExecuted { get; set; } = false;
    }
}
