using Pool;
using Prefab;

public class ParticlePooler : ObjectPooler<ParticleType>
{
    public ParticlePooler(ParticlePrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
