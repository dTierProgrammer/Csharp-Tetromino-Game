using Microsoft.Xna.Framework;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public class GroupEmitterObj: EmitterObj
{
    protected readonly EmissionSources sources; // source of emissions
    
    public GroupEmitterObj(EmissionSources sources, EmissionType emissionType)
    {
        this.sources = sources;
    }

    protected void Emit(Vector2 pos, EmitterData data)
    {
        ParticleData bufferedParticleData = data.particleData;
        bufferedParticleData.activeTime = ExtendedMath.RandomFloat(data.particleActiveTime.min, data.particleActiveTime.max);
        bufferedParticleData.speed = ExtendedMath.RandomFloat(data.speed.min, data.speed.max);
        float rand = (float)(ExtendedMath.Rng.NextDouble() * 2) - 1;
        bufferedParticleData.angle += data.angleVarianceMax * rand;

        ParticleObj bufferedParticle = new(pos, bufferedParticleData);
        ParticleManager.AddParticle(bufferedParticle);
    }

    public override void Update()
    {
        if (_emissionType != EmissionType.Burst && _isActive)
        {
            _timeInInterval -= (float)Game1.uGameTime.ElapsedGameTime.TotalSeconds;
            while (_timeInInterval <= 0)
            {
                
                foreach (var item in sources.Members)
                {
                    
                    var pos = item.Position;
                    var data = item.Data;
                    _timeInInterval = data.emissionInterval;

                    for (int i = 0; i < data.density; i++)
                    {
                      Emit(pos, data);
                    }
                }
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
                foreach (var item in sources.Members)
                {
                    var pos = item.Position;
                    var data = item.Data;

                    for (int i = 0; i < data.density; i++)
                    {
                      Emit(pos);
                    }
                }
                _isActive = false;
                _timeLeft = 0;
                break;
        }
    }
}