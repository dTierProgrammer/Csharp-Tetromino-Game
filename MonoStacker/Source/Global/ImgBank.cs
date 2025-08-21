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
    public static readonly Texture2D ClearTitle = GetContent.Load<Texture2D>("Image/Board/clear");
    public static readonly Texture2D BravoTitle = GetContent.Load<Texture2D>("Image/Board/bravo");
    public static readonly Texture2D CountDown1 = GetContent.Load<Texture2D>("Image/Board/countdown1");
    public static readonly Texture2D CountDown2 = GetContent.Load<Texture2D>("Image/Board/countdown2");
    public static readonly Texture2D CountDown3 = GetContent.Load<Texture2D>("Image/Board/countdown3");
    public static readonly Texture2D Go = GetContent.Load<Texture2D>("Image/Board/go");
}