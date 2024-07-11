using Cysharp.Threading.Tasks;
using Pool;
using Prefab;
using UnityEngine;

public class SlingshotPooler : ObjectPooler<CharacterType>
{
    public SlingshotPooler(SlingShotPrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
