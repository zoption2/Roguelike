using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public interface IUIFactory
{
    public UniTask<IEffectIconView> CreateEffectIconAsync(EffectType type, Vector3 position, Transform parent);
    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable);
    public UniTask<IAbilityIconView> CreateAbilityIconAsync(AbilityType type, Vector3 position, Transform parent);
}

public class UIFactory : IUIFactory
{
    private EffectPooler _effectPooler;
    private AbilityIconPooler _abilityIconPooler;

    [Inject]
    public void Construct(IPoolManager poolManager)
    {
        _effectPooler = poolManager.GetEffectPooler();
        _abilityIconPooler = poolManager.GetAbilityIconPooler();
    }

    public async UniTask<IEffectIconView> CreateEffectIconAsync(EffectType type, Vector3 position, Transform parent)
    {
        IEffectIconView effectIcon = await _effectPooler.PullAsync<IEffectIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable)
    {
        _effectPooler.Push(type, myPoolable);
    }

    public async UniTask<IAbilityIconView> CreateAbilityIconAsync(AbilityType type, Vector3 position, Transform parent)
    {
        IAbilityIconView abilityIcon = await _abilityIconPooler.PullAsync<IAbilityIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return abilityIcon;
    }
}
