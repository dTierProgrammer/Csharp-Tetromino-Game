using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing.PrintSupport;
using Windows.UI.Input.Inking;

namespace MonoStacker.Source.Generic.GarbageSystem
{
    public class GarbageMeter
    {
        Queue<GarbagePacket> _garbageQueued;
        private Texture2D barImage = GetContent.Load<Texture2D>("Image/Board/levelMeterFillGrey");
        private float anSegHeight = 0;
        private float segHeight = 0;
        private float prevReadyLines;
        private Color segmentColor;
        private float n_opacity;
        private float n_segHeight;

        public GarbageMeter() 
        {
            _garbageQueued = new();
        }

        public void AddGarbage(GarbagePacket garbage) 
        {
            _garbageQueued.Enqueue(garbage);
        }

        public void AddGarbage(List<int[]> garbage, float time)
        {
            var packet = new GarbagePacket(time, garbage);
            _garbageQueued.Enqueue(packet);
            segmentColor = Color.Yellow;
        }

        public int GetTotalLines() 
        {
            int num = 0;
            foreach (var item in _garbageQueued)
                num += item.GetGarbageCount();
            return num;
        }

        public int GetTotalReadyLines() 
        {
            int num = 0;
            foreach (var item in _garbageQueued)
                num += item.currentState is GarbagePacketState.Ready? item.GetGarbageCount(): 0;
            return num;
        }

        public int GetTotalPackets()
        {
            return _garbageQueued.Count;
        }

        public int GetReadyPackets() 
        {
            int num = 0;
            foreach (var item in _garbageQueued) 
            {
                if (item.currentState is GarbagePacketState.Ready)
                    num++;
            }
            return num;
        }

        private int TotalLinesBeneathId(int id) 
        {
            var current = 0;
            var num = 0;
            if (id == 0)
                return 0;
            else
                current = id - 1;

                while (current > -1)
                {
                    var item = _garbageQueued.ElementAtOrDefault(current);
                    if (item != null)
                        num += item.GetGarbageCount();
                    current--;
                }
            return num;
        }

        public int[]? DishGarbage() 
        {
            int[]? line = null;
            if (_garbageQueued.Count > 0 && _garbageQueued.Peek().currentState is GarbagePacketState.Ready) 
            {
                if (_garbageQueued.Peek().GetGarbageCount() > 0)
                    line = _garbageQueued.Peek().GetLine();
                if (_garbageQueued.Peek().GetGarbageCount() == 0)
                    _garbageQueued.Dequeue();
                segmentColor = Color.Orange;
            }
            return line;
        }

        public void NeutralizeGarbage()
        {
            
            if (_garbageQueued.Count > 0 && _garbageQueued.Peek().currentState is GarbagePacketState.Ready)
            {
                if (_garbageQueued.Peek().GetGarbageCount() > 0)
                { _garbageQueued.Peek().GetLine(); n_segHeight++; }
                if (_garbageQueued.Peek().GetGarbageCount() == 0)
                    _garbageQueued.Dequeue();
                n_opacity = 1;
                SfxBank.garbageNeutralize.Play();
            }
        }

        public void ClearQueue() 
        {
            _garbageQueued.Clear();
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var item in _garbageQueued)
                item.Update(gameTime);

            if (segHeight > GetTotalLines())
                segHeight = MathHelper.Lerp(segHeight, GetTotalLines(), .1f);
            else
                segHeight = GetTotalLines();

            if (anSegHeight < GetTotalReadyLines())
                anSegHeight = MathHelper.Lerp(anSegHeight, GetTotalReadyLines(), .1f);
            else
                anSegHeight = GetTotalReadyLines();
            
            segmentColor = Color.Lerp(segmentColor, Color.Red, .05f);
            n_opacity = MathHelper.Lerp(n_opacity, 0, .1f);
            n_segHeight = MathHelper.Lerp(n_segHeight, 0, .1f);

            prevReadyLines = GetTotalReadyLines();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 drawOffset) 
        {
           
            
            spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X,
                        (int)position.Y - (int)(segHeight * 8) + (int)drawOffset.Y,
                        3,
                        (int)(GetTotalLines() * 8)),
                    Color.Red * .2f);

            for (var i = 0; i < _garbageQueued.Count; i++) 
            {
                
                spriteBatch.Draw
                    (
                    barImage, 
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X, 
                        (int)position.Y - (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8) - (TotalLinesBeneathId(i) * 8) + (int)drawOffset.Y, 
                        1,
                        (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8)),
                        Color.Red * .08f);

                spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + 2 + (int)drawOffset.X,
                        (int)position.Y - (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8) - (TotalLinesBeneathId(i) * 8) + (int)drawOffset.Y,
                        1,
                        (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8)),
                        Color.Red * .07f);

            }
            
            /*
            spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X,
                        (int)position.Y - (int)(segHeight * 8),
                        3,
                        (int)(segHeight * 8)),
                    Color.Red * .2f);
            */
            for (var i = 0; i < _garbageQueued.Count; i++)
            {
                spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X,
                        (int)position.Y - (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8) - (TotalLinesBeneathId(i) * 8) + (int)drawOffset.Y,
                        3,
                        1),
                    Color.DarkRed);
            }

            spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X,
                        (int)position.Y - (int)(anSegHeight * 8) + (int)drawOffset.Y,
                        3,
                        (int)(anSegHeight * 8)),
                    segmentColor);

            spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X,
                        (int)position.Y - (int)(n_segHeight * 8) + (int)drawOffset.Y,
                        3,
                        (int)(n_segHeight * 8)),
                    Color.Cyan * n_opacity);

            for (var i = 0; i < _garbageQueued.Count; i++)
            {
                spriteBatch.Draw
                    (
                    barImage,
                    new Rectangle(
                        (int)position.X + (int)drawOffset.X,
                        (int)position.Y - (_garbageQueued.ElementAtOrDefault(i).GetGarbageCount() * 8) - (TotalLinesBeneathId(i) * 8) + (int)drawOffset.Y,
                        3,
                        1),
                    Color.DarkRed);
            }
          
        }
    }
}
