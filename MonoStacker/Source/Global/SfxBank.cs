using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Global
{
    public struct SfxBank
    {
        public static SoundEffect[] clear = new SoundEffect[]
        {
            GetContent.Load<SoundEffect>("Audio/Sound/clear_single"),
            GetContent.Load<SoundEffect>("Audio/Sound/clear_double"),
            GetContent.Load<SoundEffect>("Audio/Sound/clear_triple"),
            GetContent.Load<SoundEffect>("Audio/Sound/clear_quadruple")
        };

        public static SoundEffect[] clearSpin = new SoundEffect[]
        {
            GetContent.Load<SoundEffect>("Audio/Sound/spin_single"),
            GetContent.Load<SoundEffect>("Audio/Sound/spin_double"),
            GetContent.Load<SoundEffect>("Audio/Sound/spin_triple")
        };

        public static SoundEffect spinGeneric = GetContent.Load<SoundEffect>("Audio/Sound/spin_generic");
        public static SoundEffect stepHori = GetContent.Load<SoundEffect>("Audio/Sound/step_hori");
        public static SoundEffect rotate = GetContent.Load<SoundEffect>("Audio/Sound/rotate");
        public static SoundEffect rotateBuffer = GetContent.Load<SoundEffect>("Audio/Sound/rotate_buffer");
        public static SoundEffect hardDrop = GetContent.Load<SoundEffect>("Audio/Sound/hard_drop");
        public static SoundEffect softLock = GetContent.Load<SoundEffect>("Audio/Sound/soft_lock");
        public static SoundEffect hold = GetContent.Load<SoundEffect>("Audio/Sound/hold");
        public static SoundEffect holdBuffer = GetContent.Load<SoundEffect>("Audio/Sound/hold_buffer");
        public static SoundEffect b2b = GetContent.Load<SoundEffect>("Audio/Sound/b2b_streak");
        public static SoundEffect b2bBreak = GetContent.Load<SoundEffect>("Audio/Sound/b2b_break");
        public static SoundEffect lineFall = GetContent.Load<SoundEffect>("Audio/Sound/linefall");
        public static SoundEffect twist0 = GetContent.Load<SoundEffect>("Audio/Sound/twist0");
        public static SoundEffect twist1m = GetContent.Load<SoundEffect>("Audio/Sound/twist1m");
        public static SoundEffect twist1 = GetContent.Load<SoundEffect>("Audio/Sound/twist1");
    }
}
