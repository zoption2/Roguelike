using Pool;
using Prefab;
using UnityEngine;

public class EffectPooler : ObjectPooler<EffectType>
{
    private EffectPrefabHolder _provider;
    public EffectPooler(EffectPrefabHolder provider)
    {
        _provider = provider;
    }
    protected override GameObject GetPrefab(EffectType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
