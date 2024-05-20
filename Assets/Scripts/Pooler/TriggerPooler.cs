using Pool;
using Prefab;
using UnityEngine;

public class TriggerPooler : ObjectPooler<TriggerType>
{
    private BuffPrefabHolder _provider;
    public TriggerPooler(BuffPrefabHolder provider)
    {
        _provider = provider;
    }
    protected override GameObject GetPrefab(TriggerType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
