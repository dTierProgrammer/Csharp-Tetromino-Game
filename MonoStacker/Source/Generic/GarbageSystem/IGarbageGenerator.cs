using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    public interface IGarbageGenerator
    {
        public List<int[]> GenerateLines(int amount, int hole, int holeChangeChance);
    }
}
