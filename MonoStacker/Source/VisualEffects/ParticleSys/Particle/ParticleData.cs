using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle;

public struct ParticleData 
{ // Contains parameters needed to create a particle object
    public Texture2D texture { get; set; } = GetContent.Load<Texture2D>("Image/Effect/clearEffect");
    public float activeTime { get; set; } = 1; // how long the particle will be active
    public (Color color1, Color color2) colorTimeLine { get; set; } = (Color.Orange, Color.Red); // color change over time (start, end)
    public Vector2 opacityTimeLine { get; set; } = new(1f, 0f); // opacity change over time (start, end)
    public Vector2 scaleTimeLine { get; set; } = new(8, 8); // scale change over time (start, end)
    public float speed { get; set; } = 100; // particle movement speed 
    public float angle { get; set; } = 0f;// particle movement angle
    public float rotationSpeed { get; set; } // particle rotation speed
    
    // TODO: velocity, accel, friction modifiers to simulate physics
    
    public ParticleData() { }
}