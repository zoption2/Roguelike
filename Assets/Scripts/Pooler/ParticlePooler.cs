using Cysharp.Threading.Tasks;
using Pool;
using Prefab;
using UnityEngine;

public class ParticlePooler : ObjectPooler<ParticleType>
{
    public ParticlePooler(ParticlePrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
