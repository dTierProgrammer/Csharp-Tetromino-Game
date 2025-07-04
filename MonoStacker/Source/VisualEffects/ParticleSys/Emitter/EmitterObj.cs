using System;
using Microsoft.Xna.Framework;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public enum EmissionType
{
    Continuous = 0,
    Timed = 1,
    Burst = 2
}

public class EmitterObj
{ // Object that will emit particles
    protected readonly EmitterData _data; // given interval data
    protected float _timeInInterval; // time leftover within an interval
    protected readonly EmissionSource EmissionSource; //source of emission position
    public EmissionType _emissionType;
    public float _timeLeft;
    public bool _isActive = true;

    public EmitterObj(EmissionSource emissionSource, EmitterData data, EmissionType emissionType)
    {
        EmissionSource = emissionSource;
        _data = data;
        _timeInInterval = data.emissionInterval;
        _emissionType = emissionType;
        _timeLeft = data.activeTimeLeft;
    }

    public EmitterObj(EmitterData data, EmissionType emissionType)
    {
        _data = data;
        _timeInInterval = data.emissionInterval;
        _emissionType = emissionType;
        _timeLeft = data.activeTimeLeft;
    }

    public EmitterObj() { }

    protected void Emit(Vector2 pos)
    {
        ParticleData bufferedParticleData = _data.particleData;
        bufferedParticleData.activeTime = ExtendedMath.RandomFloat(_data.particleActiveTime.min, _data.particleActiveTime.max);
        bufferedParticleData.speed = ExtendedMath.RandomFloat(_data.speed.min, _data.speed.max);
        float rand = (float)(ExtendedMath.Rng.NextDouble() * 2) - 1;
        bufferedParticleData.angle += _data.angleVarianceMax * rand;

        ParticleObj bufferedParticle = new(pos, bufferedParticleData);
        ParticleManager.AddParticle(bufferedParticle);
    }

    public virtual void Update()
    {
        
        if (_emissionType != EmissionType.Burst && _isActive)
        {
            _timeInInterval -= (float)Game1.uGameTime.ElapsedGameTime.TotalSeconds;
            while (_timeInInterval <= 0)
            {
                _timeInInterval = _data.emissionInterval;
                var pos = EmissionSource.Position;
                for(int i = 0; i < _data.density; i++)
                    Emit(pos);
            }
        }
        
        switch (_emissionType)
        {
            case EmissionType.Continuous:
                break;
            case EmissionType.Timed:
                _timeLeft -= (float)Game1.uGameTime.ElapsedGameTime.TotalSeconds;
                if (_timeLeft <= 0)
                    _isActive = false;
                break;
            case EmissionType.Burst:
            {
                var pos = EmissionSource.Position;
                for (int i = 0; i < _data.density; i++)
                    Emit(pos);
                _isActive = false;
                break;
            }
        }
    }
}