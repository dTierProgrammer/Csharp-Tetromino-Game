using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;

namespace MonoStacker.Source.Generic.GarbageSystem.Factory
{
    internal class StandardGarbageGenerator: IGarbageGenerator
    {
        public List<int[]> GenerateLines(int amount, int hole, int holeChangeChance) 
        {
            List<int[]> garbage = new();
            while (hole < 0) 
            {
                hole++;
            }

            for (var i = 0; i < amount; i++)
            {
                var chance = ExtendedMath.Rng.Next(1, 100);


                if (chance <= holeChangeChance)
                {
                    var offset = 0;
                    var sideChance = ExtendedMath.Rng.Next(100);
                    if (sideChance <= 50)
                        offset++;
                    else
                        offset--;
                    if (hole + offset > Grid.COLUMNS - 1 || hole + offset < 0)
                        offset *= -1;
                    garbage.Add(LineFactory.CreateLine(hole + offset, 0, 8));
                }
                else
                    garbage.Add(LineFactory.CreateLine(hole , 0, 8));
            }
            return garbage;
        }
    }
}
