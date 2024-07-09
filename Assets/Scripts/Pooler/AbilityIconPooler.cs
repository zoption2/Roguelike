using Cysharp.Threading.Tasks;
using Pool;
using Unity.AI.Navigation;
using UnityEngine;

public class AbilityIconPooler : ObjectPooler<AbilityType>
{
    public AbilityIconPooler(UIAbilitiesPrefabHolder provider)
    {
        _prefabHolder = provider;
    }
}
