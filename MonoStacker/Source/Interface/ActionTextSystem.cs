using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.Text;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Interface
{
    public enum ActionTextLayout 
    {
        Horizontal,
        Vertical
    }

    enum ComboState 
    {
        Active,
        Inactive
    }

    enum B2bState 
    {
        Active,
        Inactive
    }

    public class ActionTextSystem
    {
        private List<Flair> _textList;
        private Vector2 _offset;

        public ActionTextSystem(Vector2 offset) 
        {
            _textList = new();
            _offset = offset;
        }

        public void Ping(string text, Color startColor, Color endColor, float timeDisplayed, float fadeOutTime) 
        {
            _textList.Insert(0, new(Font.DefaultSmallOutlineGradient_Alt, text, timeDisplayed, fadeOutTime, startColor, endColor, OriginSetting.TopRight));
        }

        public void Update(GameTime gameTime, PlayField playField) 
        {
                foreach (var item in _textList)
                    item.Update(gameTime);
            _textList.RemoveAll(text => text.fadeOutTime <= 0);

        }

        public void Draw(SpriteBatch spriteBatch, PlayField playField) 
        {

            for (var i = 0; i < _textList.Count; i++) 
            {
                if(i < 4)
                    _textList[i].Draw(spriteBatch, new Vector2(_offset.X, _offset.Y + 9 + (i * 8)));
            }
                    
        }
    }
}
