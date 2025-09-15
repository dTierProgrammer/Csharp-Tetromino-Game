using MonoStacker.Source.Generic.Rotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    public interface IAttackSystem
    {
        public List<int[]> BuildAttackLines(int linesCleared, int combo, int streak, SpinType spinType, int currentOffset, IGarbageGenerator generator);
    }
}
