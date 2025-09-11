using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    internal class GarbageSys
    {
        private Queue<GarbagePacket> _packets;
        private int lineRecieveLimit = 12;

        public GarbageSys() 
        {
            _packets = new();
        }

        public int GetTotalGarbageLines() 
        {
            var num = 0;
            foreach (var item in _packets) 
            {
                foreach (var line in item.garbage)
                    num++;
            }
            return num;
        }

        public void QueueGarbage(List<int[]> lines) 
        {
            _packets.Enqueue(new GarbagePacket(lines));
        }

        public void NeutralizeGarbage(List<int[]> lines) 
        {
            for (var i = 0; i < lines.Count; i++) 
            {
                _packets.Peek().garbage.Remove(_packets.Peek().garbage[i]);
                if (_packets.Peek().garbage.Count == 0)
                    _packets.Dequeue();
            }
        }

        public GarbagePacket RecieveDamage() 
        {
            return _packets.Dequeue();
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var packet in _packets)
                packet.Update(gameTime);
        }
    }
}
