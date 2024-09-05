using Pool;
using Prefab;

public class UIPooler : ObjectPooler<UIElementType>
{
    public UIPooler(UIPrefabHolder holder)
    {
        _prefabHolder = holder;
    }
}
