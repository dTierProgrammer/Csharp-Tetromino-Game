using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RasterFontLibrary.Source;
using System.Collections.Generic;

namespace MonoStacker.Source.Global;

public struct ImgBank
{
    public static readonly Texture2D BlockTexture = GetContent.Load<Texture2D>("Image/Block/0");
    public static Dictionary<int, Rectangle> BlockCuts = new();
    
    public static readonly Texture2D GhostBlockTexture = GetContent.Load<Texture2D>("Image/Block/0gp");
    public static readonly Texture2D GridBg = GetContent.Load<Texture2D>("Image/Board/grid");
    public static readonly Texture2D BoardBg = GetContent.Load<Texture2D>("Image/Board/bg_gradient");
}