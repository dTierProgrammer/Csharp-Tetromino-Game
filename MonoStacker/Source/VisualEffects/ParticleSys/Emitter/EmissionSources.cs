using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public interface EmissionSources
{
    public List<GroupPartData> Members { get; set; }
}