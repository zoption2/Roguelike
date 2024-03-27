using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPooler : ObjectPooler<BuffType>
{
    private BuffPrefabHolder _provider;
    public BuffPooler(BuffPrefabHolder provider)
    {
        _provider = provider;
    }
    protected override GameObject GetPrefab(BuffType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
