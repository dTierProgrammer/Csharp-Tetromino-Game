using Microsoft.Xna.Framework.Graphics;

namespace MonoStacker.Source.Global;

public static class ImgBank
{
    public static readonly Texture2D BlockTexture = GetContent.Load<Texture2D>("Image/Block/1");
    public static readonly Texture2D GhostBlockTexture = GetContent.Load<Texture2D>("Image/Block/0gp");
    public static readonly Texture2D GridBg = GetContent.Load<Texture2D>("Image/Board/grid");
    public static readonly Texture2D BoardBg = GetContent.Load<Texture2D>("Image/Board/bg_gradient");
}