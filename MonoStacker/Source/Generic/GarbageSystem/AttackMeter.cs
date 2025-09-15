using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    public class AttackMeter
    {
        private List<int[]> attack;
        private static Texture2D meterFill = GetContent.Load<Texture2D>("Image/Board/levelMeterFillGrey");
        private float segHeight;
        private float anSegHeight;


        public AttackMeter() 
        {
            attack = new();
        }

        public void ChargeAttack(List<int[]> attack) 
        {
            this.attack.AddRange(attack);
            segHeight = this.attack.Count;
        }

        public int GetCount() 
        {
            return attack.Count;
        }

        public void RemoveLineFromAttack() 
        {
            if (attack.Count > 0)
                attack.RemoveAt(0);
            segHeight = this.attack.Count;
        }

        public void SendAttack(GarbageMeter garbageSys, float time) 
        {
            if (attack.Count <= 0) return;
            garbageSys.AddGarbage(attack, time);
            
            ClearAttack();
            segHeight = this.attack.Count;
        }

        public void ClearAttack() 
        {
            attack.Clear();
            segHeight = attack.Count;
        }

        public void Update(GameTime gameTime) 
        {
            if (anSegHeight < segHeight)
                anSegHeight = MathHelper.Lerp(anSegHeight, segHeight, .1f);
            else
                anSegHeight = segHeight;
            //segHeight = attack.Count;
            //Debug.WriteLine(attack.Count);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 drawOffset) 
        {
            spriteBatch.Draw
                (
                meterFill,
                new Rectangle(
                    (int)position.X + (int)drawOffset.X,
                    (int)position.Y - (int)(segHeight * 8),
                    3,
                    (int)segHeight * 8
                    ),
                Color.Orange
                );

            spriteBatch.Draw
                (
                meterFill,
                new Rectangle(
                    (int)position.X + (int)drawOffset.X,
                    (int)position.Y - (int)(anSegHeight * 8),
                    3,
                    (int)anSegHeight * 8
                    ),
                Color.Yellow
                );
        }
    }
}
