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
    public void Construct(EffectPooler pooler, AbilityIconPooler abilityIconPooler)
    {
        _effectPooler = pooler;
        _effectPooler.Init();

        _abilityIconPooler = abilityIconPooler;
        _abilityIconPooler.Init();
    }

    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent)
    {
        IEffectIconView effectIcon = _effectPooler.Pull<IEffectIconView>(type, position, Quaternion.identity, parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable)
    {
        _effectPooler.Push(type, myPoolable);
    }

    public IAbilityIconView CreateAbilityIcon(AbilityType type, Vector3 position, Transform parent)
    {
        IAbilityIconView abilityIcon = _abilityIconPooler.Pull<IAbilityIconView>(type, position, Quaternion.identity, parent);
        return abilityIcon;
    }
}
