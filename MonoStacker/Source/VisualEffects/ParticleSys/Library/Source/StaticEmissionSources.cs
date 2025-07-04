using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;

public class StaticEmissionSources(List<GroupPartData> members) : EmissionSources
{
    public List<GroupPartData> Members { get; set; } = members;
    
}