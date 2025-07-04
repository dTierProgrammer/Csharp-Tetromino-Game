using MonoStacker.Source.GameObj.Tetromino;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Emitter;

public class MultiEmitterObj: EmitterObj
{
    protected readonly EmissionSources sources; // source of emissions
    
    public MultiEmitterObj(EmissionSources sources, EmitterData data, EmissionType emissionType): base(data, emissionType)
    {
        this.sources = sources;
    }

    public override void Update()
    {
        if (_emissionType != EmissionType.Burst && _isActive)
        {
            _timeInInterval -= (float)Game1.uGameTime.ElapsedGameTime.TotalSeconds;
            while (_timeInInterval <= 0)
            {
                _timeInInterval = _data.emissionInterval;
                
                foreach (var item in sources.Positions) 
                {
                    var pos = item;
                    for (int i = 0; i < _data.density; i++)
                        Emit(pos);
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
                foreach (var item in sources.Positions)
                {
                    var pos = item;
                    for (int i = 0; i < _data.density; i++)
                        Emit(pos);
                }
                _isActive = false;
                break;
        }
    }
}