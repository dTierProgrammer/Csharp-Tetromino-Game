using Microsoft.Xna.Framework;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using System.Diagnostics;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public class GroupEmitterObj: EmitterObj
{
    protected readonly EmissionSources sources; // source of emissions
    
    public GroupEmitterObj(EmissionSources sources, EmissionType emissionType)
    {
        this.sources = sources;
        _emissionType = emissionType;
    }

    protected void Emit(Vector2 pos, EmitterData data)
    {
        ParticleData bufferedParticleData = data.particleData;
        bufferedParticleData.activeTime = ExtendedMath.RandomFloat(data.particleActiveTime.min, data.particleActiveTime.max);
        bufferedParticleData.speed = ExtendedMath.RandomFloat(data.speed.min, data.speed.max);
        float rand = (float)(ExtendedMath.Rng.NextDouble() * 2) - 1;
        bufferedParticleData.angle += data.angleVarianceMax * rand;
        emissionState = EmitterState.Active;

        ParticleObj bufferedParticle = new(pos, bufferedParticleData);
        ParticleManager.AddParticle(bufferedParticle);
    }

    protected override void UpdateBurst() 
    {
        foreach (var item in sources.Members)
        {
            var pos = item.Position;
            var data = item.Data;
            _timeInInterval = data.emissionInterval;
            for (int i = 0; i < data.density; i++)
                Emit(pos, data);
        }
        emissionState = EmitterState.Inactive;
    }

    protected override void UpdateTimed(GameTime gameTime) 
    {
        while (_timeInInterval <= 0)
        {
            foreach (var item in sources.Members)
            {
                var pos = item.Position;
                var data = item.Data;
                _timeInInterval = data.emissionInterval;
                for (int i = 0; i < data.density; i++)
                    Emit(pos, data);
            }
        }
        _timeInInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeLeft <= 0) { emissionState = EmitterState.Inactive; return; }
        _timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected override void UpdateContinuous(GameTime gameTime) 
    {
        while (_timeInInterval <= 0)
        {
            foreach (var item in sources.Members)
            {
                var pos = item.Position;
                var data = item.Data;
                _timeInInterval = data.emissionInterval;
                for (int i = 0; i < data.density; i++)
                    Emit(pos, data);
            }
        }
        _timeInInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public override void Update(GameTime gameTime)
    {
        switch (_emissionType) 
        {
            case EmissionType.Continuous:
                UpdateContinuous(gameTime);
                break;
            case EmissionType.Timed:
                UpdateTimed(gameTime);
                break;
            case EmissionType.Burst:
                UpdateBurst();
                break;
        }
    }
}