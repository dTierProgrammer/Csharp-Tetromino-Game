using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStacker.Source.VisualEffects;

public static class EffectManager // may try to merge with particle system
{
    private static readonly List<VisualEffect> _visualEffects = new();

    public static void AddEffect(VisualEffect effect)
    {
        _visualEffects.Add(effect);
    }

    public static void AddEffects(List<VisualEffect> effects)
    {
        _visualEffects.AddRange(effects);
    }

    public static void Update(GameTime gameTime)
    {
        foreach (var effect in _visualEffects)
            effect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        foreach (var effect in _visualEffects)
            effect.Draw(spriteBatch);
        spriteBatch.End();
    }
}