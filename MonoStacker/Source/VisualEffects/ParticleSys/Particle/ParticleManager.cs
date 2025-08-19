using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle;

public class ParticleManager
{ // manager class for particles, emitters
    private static readonly List<ParticleObj> _particles = new(); // every particle instance
    private static readonly List<EmitterObj> _emitters = new(); // every emissionSource instance
    private static readonly List<EmitterObj> _emitterKillList = new();

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

    private static void UpdateParticles(GameTime gameTime)
    {
        foreach (var particle in _particles)
            particle.Update(gameTime);
        _particles.RemoveAll(particle => particle.activeTimeLeft <= 0);
    }

    private static void UpdateEmitters(GameTime gameTime)
    {
        foreach (var emitter in _emitters) 
        {
            if (emitter._emitParticles) 
                emitter.Update(gameTime);
        }
        _emitters.RemoveAll(emitter => emitter.emissionState == EmitterState.Inactive);
    }

    public static void Update(GameTime gameTime)
    {
        UpdateParticles(gameTime);
        UpdateEmitters(gameTime);;
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        foreach (var particle in _particles)
            particle.Draw(spriteBatch);
        spriteBatch.End();
    }
}