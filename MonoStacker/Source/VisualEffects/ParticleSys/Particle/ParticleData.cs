using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle;

public struct ParticleData 
{ // Contains parameters needed to create a particle object
    public Texture2D texture { get; set; }
    public float activeTime { get; set; } // how long the particle will be active
    public (Color color1, Color color2) colorTimeLine { get; set; } // color change over time (start, end)
    public Vector2 opacityTimeLine { get; set; } // opacity change over time (start, end)
    public Vector2 scaleTimeLine { get; set; } // scale change over time (start, end)
    public float speed { get; set; } // particle movement speed
    public float angle { get; set; } // particle movement angle
    public float rotationSpeed { get; set; } // particle rotation speed
    
    // TODO: velocity, accel, friction modifiers to simulate physics
}