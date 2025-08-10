using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoStacker.Source.VisualEffects;

public class AnimatedEffectLayer
{
    private  readonly List<AnimatedEffect> _visualEffects = new();

    public  void AddEffect(AnimatedEffect effect)
    {
        _visualEffects.Add(effect);
    }

    public  void AddEffects(List<AnimatedEffect> effects)
    {
        _visualEffects.AddRange(effects);
    }

    public  void Update(GameTime gameTime)
    {
        foreach (var effect in _visualEffects)
            effect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public  void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.Begin();
        foreach (var effect in _visualEffects)
            effect.Draw(spriteBatch);
        //spriteBatch.End();
    }
}