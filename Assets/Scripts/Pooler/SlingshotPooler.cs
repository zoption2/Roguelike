using Pool;
using Prefab;
using UnityEngine;

public class SlingshotPooler : ObjectPooler<CharacterType>
{
    private SlingShotPrefabHolder _provider;
    public SlingshotPooler(SlingShotPrefabHolder provider)
    {
        _provider = provider;
    }
    protected override GameObject GetPrefab(CharacterType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
