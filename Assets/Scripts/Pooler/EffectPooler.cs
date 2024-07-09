using Cysharp.Threading.Tasks;
using Pool;
using Prefab;
using UnityEngine;

public class EffectPooler : ObjectPooler<EffectType>
{
    public EffectPooler(EffectPrefabHolder provider)
    {
        _prefabHolder = provider;
    }
}
