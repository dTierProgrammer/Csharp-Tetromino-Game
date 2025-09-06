using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle
{
    public class ParticleLayer
    { // individual layer for if individual objects need to emit particles 
        private  readonly List<ParticleObj> _particles = new(); // every particle instance
        private  readonly List<EmitterObj> _emitters = new(); // every emissionSource instance

        public void AddParticle(ParticleObj particle) // add single particle to particles list
        {
            _particles.Add(particle);
        }

        public void AddParticles(List<ParticleObj> particles) // add multiple particles to list (hard code each)
        {
            _particles.AddRange(particles);
        }

        public void AddEmitter(EmitterObj emitter) // add single emissionSource to emissionSource list
        {
            _emitters.Add(emitter);
        }

        public void AddEmitters(List<EmitterObj> emitters) // add multiple emitters to emissionSource list
        {
            _emitters.AddRange(emitters);
        }

        private void UpdateParticles(GameTime gameTime)
        {
            foreach (var particle in _particles)
                particle.Update(gameTime);
            _particles.RemoveAll(particle => particle.activeTimeLeft <= 0);
        }

        private void UpdateEmitters(GameTime gameTime)
        {
            foreach (var emitter in _emitters)
            {
                if (emitter._emitParticles)
                    emitter.Update(gameTime);
            }
            _emitters.RemoveAll(emitter => emitter.emissionState == EmitterState.Inactive);
        }

        public void Update(GameTime gameTime)
        {
            UpdateParticles(gameTime);
            UpdateEmitters(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
                particle.Draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, float layer)
        {
            foreach (var particle in _particles)
                particle.Draw(spriteBatch, layer);
        }
    }
}
