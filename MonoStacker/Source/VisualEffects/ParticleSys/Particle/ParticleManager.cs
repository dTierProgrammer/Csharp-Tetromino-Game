using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle;

public class ParticleManager
{ // manager class for particles, emitters
    private static readonly List<ParticleObj> _particles = new(); // every particle instance
    private static readonly List<EmitterObj> _emitters = new(); // every emissionSource instance

    public static void AddParticle(ParticleObj particle) // add single particle to particles list
    {
        _particles.Add(particle);
    }

    public static void AddParticles(List<ParticleObj> particles) // add multiple particles to list (hard code each)
    {
        _particles.AddRange(particles);
    }

    public static void AddEmitter(EmitterObj emitter) // add single emissionSource to emissionSource list
    {
        _emitters.Add(emitter);
    }

    public static void AddEmitters(List<EmitterObj> emitters) // add multiple emitters to emissionSource list
    {
        _emitters.AddRange(emitters);
    }

    private static void UpdateParticles()
    {
        foreach (var particle in _particles)
            particle.Update(Game1.uGameTime);
        _particles.RemoveAll(particle => particle.activeTimeLeft <= 0);
    }

    private static void UpdateEmitters()
    {
        foreach (var emitter in _emitters)
            emitter.Update();
        _emitters.RemoveAll(emitter => emitter._timeLeft <= 0);
    }

    public static void Update()
    {
        UpdateParticles();
        UpdateEmitters();
        _emitters.RemoveAll(emitter => emitter._emissionType == EmissionType.Burst && emitter._timeLeft <= 0);
        //Debug.WriteLine(_particles.Count);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        foreach (var particle in _particles)
            particle.Draw(spriteBatch);
        spriteBatch.End();
    }
}