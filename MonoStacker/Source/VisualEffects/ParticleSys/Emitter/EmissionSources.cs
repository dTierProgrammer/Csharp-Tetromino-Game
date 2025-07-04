using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public interface EmissionSources
{
    public List<Vector2> Positions { get; }
}