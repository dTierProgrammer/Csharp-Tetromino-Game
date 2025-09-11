using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.Rotation
{
    public interface ICombatSystem
    {
        public List<int[]> Attack(int linesCleared, SpinType spinType);
        public List<int[]> Defend(int linesCleared, SpinType spinType);
    }
}
