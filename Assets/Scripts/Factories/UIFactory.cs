using UnityEngine;
using Zenject;

public interface IUIFactory
{
    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent);
    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable);
    public IAbilityIconView CreateAbilityIcon(AbilityType type, Vector3 position, Transform parent);
}

public class UIFactory : IUIFactory
{
    private EffectPooler _effectPooler;
    private AbilityIconPooler _abilityIconPooler;

    [Inject]
    public void Construct(IPoolManager poolManager)
    {
        _effectPooler = poolManager.UseEffectPooler();
        _abilityIconPooler = poolManager.UseAbilityIconPooler();
    }

    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent)
    {
        IEffectIconView effectIcon = _effectPooler.Pull<IEffectIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable)
    {
        _effectPooler.Push(type, myPoolable);
    }

    public IAbilityIconView CreateAbilityIcon(AbilityType type, Vector3 position, Transform parent)
    {
        IAbilityIconView abilityIcon = _abilityIconPooler.Pull<IAbilityIconView>(type, position, Quaternion.Euler(90, 0, 0), parent);
        return abilityIcon;
    }
}
