using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardUIPooler : ObjectPooler<RewardType>
{
    public RewardUIPooler(RewardUIPrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
