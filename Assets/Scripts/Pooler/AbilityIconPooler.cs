using Pool;
using UnityEngine;

public class AbilityIconPooler : ObjectPooler<AbilityType>
{
    private UIAbilitiesPrefabHolder _provider;
    public AbilityIconPooler(UIAbilitiesPrefabHolder provider)
    {
        _provider = provider;
    }
    protected override GameObject GetPrefab(AbilityType tag)
    {
        return _provider.GetPrefab(tag);
    }
}
