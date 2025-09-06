using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public class EmitterData // Conatins parameters emitters will emit particles based off
{
    public ParticleData particleData { get; set; } // given particle data
    public float angle { get; set; } = 0f; // angle of an emitted particle
    public float angleVarianceMax; // the maximum angle an emitted particle can travel in
    public (float min, float max) particleActiveTime { get; set; } // the minimum and maximum time a particle can be active for
    public (float min, float max) speed { get; set; } // the minimum and maximum speed an emitted particle can travel at
    public float emissionInterval; // the time intervals particles will be emitted at
    public int density; // how many particles will be emitted every interval
    public float activeTimeLeft { get; set; } // (if emissionSource is marked as timed) how long the emissionSource will stay active (different from emissionInterval)
    public (float min, float max) offsetX { get; set; } = (0, 0);
    public (float min, float max) offsetY { get; set; } = (0, 0);

    public (float min, float max) rotationSpeed = (0, 0);

    public EmitterData() { }

}