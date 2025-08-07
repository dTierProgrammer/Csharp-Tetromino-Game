using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using System;
using System.Collections.Generic;
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

        private void UpdateEmitters()
        {
            foreach (var emitter in _emitters)
                emitter.Update();
            _emitters.RemoveAll(emitter => emitter._timeLeft <= 0);
        }

        public void Update(GameTime gameTime)
        {
            UpdateParticles(gameTime);
            UpdateEmitters();
            _emitters.RemoveAll(emitter => emitter._emissionType == EmissionType.Burst && emitter._timeLeft <= 0);
            //Debug.WriteLine(_particles.Count);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (var particle in _particles)
                particle.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
