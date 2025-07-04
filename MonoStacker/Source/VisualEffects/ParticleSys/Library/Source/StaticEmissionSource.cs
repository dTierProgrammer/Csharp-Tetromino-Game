using Microsoft.Xna.Framework;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;

public class StaticEmissionSource(Vector2 position) : EmissionSource
{
    public Vector2 Position { get; set; } = position;
}