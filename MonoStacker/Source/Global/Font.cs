using Microsoft.Xna.Framework.Graphics;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Global
{
    public struct Font
    {
        public static RasterFont DefaultSmallOutlineGradient = new(GetContent.Load<Texture2D>("Image/Font/small_outline"), 1, 1, -1);
        public static RasterFont DefaultSmallOutlineGradient_Alt = new(GetContent.Load<Texture2D>("Image/Font/small_outline_1"), 1, 1, -1);
        public static RasterFont SmallSquareSmall = new(GetContent.Load<Texture2D>("Image/Font/square"), 1, 1, 1);
        public static RasterFont SmallSquareGradientSmall = new(GetContent.Load<Texture2D>("Image/Font/square_gradient"), 1, 1, 1);
    }
}
