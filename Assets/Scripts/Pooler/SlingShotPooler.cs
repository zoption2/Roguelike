using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotPooler : ObjectPooler<SlingShotType>
{
    private SlingShotPrefabHolder _provider;
    public SlingshotPooler(SlingShotPrefabHolder provider)
    {
        _provider = provider;
    }

    protected override GameObject GetPrefab(SlingShotType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
