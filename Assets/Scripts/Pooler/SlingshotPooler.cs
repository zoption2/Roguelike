using Cysharp.Threading.Tasks;
using Pool;
using Prefab;
using UnityEngine;

public class SlingshotPooler : ObjectPooler<CharacterType>
{
    public SlingshotPooler(SlingShotPrefabHolder provider)
    {
        _prefabHolder = provider;
    }
}
