using Microsoft.Xna.Framework;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    public enum GarbagePacketState 
    {
        Waiting,
        Ready
    }
    public class GarbagePacket
    {
        public List<int[]> garbage = new();
        public float time { get; private set; }
        public GarbagePacketState currentState { get; private set; }
        

        public GarbagePacket(float time, List<int[]>garbage) 
        {
            //this.garbage = garbage;
            this.garbage.AddRange(garbage);
            this.time = time;
            currentState = GarbagePacketState.Waiting;
        }

        private void PlaySound() 
        {
            if (garbage.Count >= 12)
                SfxBank.garbageHitVeryLarge.Play();
            else if(garbage.Count < 12 && garbage.Count >= 9)
                SfxBank.garbageHitLarge.Play();
            else if (garbage.Count < 9 && garbage.Count >= 6)
                SfxBank.garbageHitMedium.Play();
            else if (garbage.Count < 6 && garbage.Count >= 3)
                SfxBank.garbageHitSmall.Play();
            else if (garbage.Count <= 3)
                SfxBank.garbageHitGeneric.Play();
        }

        public int GetGarbageCount() 
        {
            return garbage.Count;
        }

        public int[] GetLine() 
        {
            int[] line = garbage.ElementAt(0);
            garbage.RemoveAt(0);
            return line;
        }

        public void Update(GameTime gameTime) 
        {
            if (time <= 0) 
            {
                if(currentState == GarbagePacketState.Waiting)
                    PlaySound();
                currentState = GarbagePacketState.Ready;
                
            }
                
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
