using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotPooler : ObjectPooler<PlayerType>
{
    private SlingShotPrefabHolder _provider;
    public SlingshotPooler(SlingShotPrefabHolder provider)
    {
        _provider = provider;
    }

    protected override GameObject GetPrefab(PlayerType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
