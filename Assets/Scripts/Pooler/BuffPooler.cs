using Cysharp.Threading.Tasks;
using Pool;
using Prefab;
using UnityEngine;

public class BuffPooler : ObjectPooler<BuffType>
{
    public BuffPooler(BuffPrefabHolder provider)
    {
        _prefabHolder = provider;
    }
}
