using Pool;
using Prefab;

public class SlingshotPooler : ObjectPooler<CharacterType>
{
    public SlingshotPooler(SlingShotPrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
