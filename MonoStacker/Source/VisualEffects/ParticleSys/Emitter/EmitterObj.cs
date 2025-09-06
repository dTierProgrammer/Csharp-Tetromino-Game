using System;
using System.Diagnostics;
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

public enum EmitterState 
{
    Active,
    Inactive
}

public class EmitterObj
{ // Object that will emit particles
    protected readonly EmitterData _data; // given interval data
    protected float _timeInInterval; // time leftover within an interval
    protected readonly EmissionSource EmissionSource; //source of emission position
    public EmissionType _emissionType;
    public EmitterState emissionState;
    public float _timeLeft;
    public bool _emitParticles = true;

    public EmitterObj(EmissionSource emissionSource, EmitterData data, EmissionType emissionType)
    {
        EmissionSource = emissionSource;
        _data = data;
        _timeInInterval = data.emissionInterval;
        _emissionType = emissionType;
        _timeLeft = data.activeTimeLeft;
        emissionState = EmitterState.Active;
    }

    public EmitterObj(EmissionSource emissionSource, EmitterData data, EmissionType emissionType, bool isActive)
    {
        EmissionSource = emissionSource;
        _data = data;
        _timeInInterval = data.emissionInterval;
        _emissionType = emissionType;
        _timeLeft = data.activeTimeLeft;
        _emitParticles = isActive;
        emissionState = EmitterState.Active;
    }

    public EmitterObj(EmitterData data, EmissionType emissionType)
    {
        _data = data;
        _timeInInterval = data.emissionInterval;
        _emissionType = emissionType;
        _timeLeft = data.activeTimeLeft;
        emissionState = EmitterState.Active;
    }

    public EmitterObj() { }

    protected void Emit(Vector2 pos)
    {
        var bufferedParticleData = _data.particleData;
        bufferedParticleData.activeTime = ExtendedMath.RandomFloat(_data.particleActiveTime.min, _data.particleActiveTime.max);
        bufferedParticleData.speed = ExtendedMath.RandomFloat(_data.speed.min, _data.speed.max);
        float rand = (float)(ExtendedMath.Rng.NextDouble() * 2) - 1;
        bufferedParticleData.angle += _data.angleVarianceMax * rand;
        bufferedParticleData.offset.X += ExtendedMath.RandomFloat(_data.offsetX.min, _data.offsetX.max);
        bufferedParticleData.offset.Y += ExtendedMath.RandomFloat(_data.offsetY.min, _data.offsetY.max);
        bufferedParticleData.rotationSpeed += ExtendedMath.RandomFloat(_data.rotationSpeed.min, _data.rotationSpeed.max);

        ParticleObj bufferedParticle = new(pos, bufferedParticleData);
        ParticleManager.AddParticle(bufferedParticle);
    }

    protected virtual void UpdateBurst()
    {
        emissionState = EmitterState.Inactive;
        var pos = EmissionSource.Position;
        for (int i = 0; i < _data.density; i++)
            Emit(pos);
        return;
    }

    protected virtual void UpdateTimed(GameTime gameTime)
    {
        while (_timeInInterval <= 0)
        {
            _timeInInterval = _data.emissionInterval;
            var pos = EmissionSource.Position;
            for (int i = 0; i < _data.density; i++)
                Emit(pos);
        }
        _timeInInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeLeft <= 0) { emissionState = EmitterState.Inactive; return; }
        _timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected virtual void UpdateContinuous(GameTime gameTime)
    {
        while (_timeInInterval <= 0)
        {
            _timeInInterval = _data.emissionInterval;
            var pos = EmissionSource.Position;
            for (int i = 0; i < _data.density; i++)
                Emit(pos);
        }
        _timeInInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public virtual void Update(GameTime gameTime)
    {
        switch (_emissionType) 
        {
            case EmissionType.Burst:
                UpdateBurst();
                break;
            case EmissionType.Timed:
                UpdateTimed(gameTime);
                break;
            case EmissionType.Continuous:
                UpdateContinuous(gameTime);
                break;
        }
    }
}