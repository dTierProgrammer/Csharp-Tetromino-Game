using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    enum GarbagePacketState 
    {
        Startup,
        PreActive,
        Active
    }
    public class GarbagePacket
    {
        public List<int[]> garbage;
        public float startupTime;
        public float activationTime;
        private GarbagePacketState _currentState = GarbagePacketState.Startup;

        public GarbagePacket(List<int[]> lines) 
        {
            garbage = new();
            garbage.AddRange(lines);
        }

        public void Update(GameTime gameTime) 
        {
            switch(_currentState)
            {
                case GarbagePacketState.Startup:
                    if (startupTime <= 0)
                        _currentState = GarbagePacketState.PreActive;
                    startupTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case GarbagePacketState.PreActive:
                    if (activationTime <= 0)
                        _currentState = GarbagePacketState.Active;
                    activationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }
        }
    }
}
