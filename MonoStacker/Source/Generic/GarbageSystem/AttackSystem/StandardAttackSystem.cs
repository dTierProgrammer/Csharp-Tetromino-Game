using MonoStacker.Source.Generic.Rotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem.AttackSystem
{
    internal class StandardAttackSystem: IAttackSystem
    {
        public List<int[]> BuildAttackLines(int linesCleared, int combo, int streak, SpinType spinType, int currentOffset, IGarbageGenerator generator) 
        {
            var send = 0;
            if (spinType == SpinType.None)
            {
                send += linesCleared switch
                {
                    1 => 0,
                    2 => 1,
                    3 => 2,
                    4 => 4,
                    _ => 4
                } + ComboMultiplier(combo);
            }
            else if (spinType == SpinType.MiniSpin)
            {
                send += linesCleared switch
                {
                    1 => 0,
                    2 => 1,
                    3 => 2,
                    4 => 4,
                    _ => 4
                } + ComboMultiplier(combo);
            }
            else
            {
                send += linesCleared switch
                {
                    1 => 2,
                    2 => 4,
                    3 => 6,
                    4 => 10,
                    _ => 10
                } + ComboMultiplier(combo);
            }
            return generator.GenerateLines(send, currentOffset, 30);
        }

        private int ComboMultiplier(int combo)
        {
            return combo switch
            {
                -1 => 0,
                0 => 0,
                1 => 0,
                2 => 1,
                3 => 1,
                4 => 2,
                5 => 2,
                6 => 3,
                7 => 3,
                8 => 4,
                9 => 4,
                10 => 4,
                11 => 5,
                12 => 5,
                13 => 5,
                14 => 6,
                15 => 6,
                16 => 6,
                17 => 7,
                18 => 8,
                _ => 8
            };
        }
    }
}
