using Pool;
using UnityEngine;
using Zenject;

public interface IUIFactory
{
    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent);
    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable);
}

public class UIFactory : IUIFactory
{
    
    private EffectPooler _pooler;

    [Inject]
    public void Construct(EffectPooler pooler)
    {
        _pooler = pooler;
        _pooler.Init();
    }

    public IEffectIconView CreateEffectIcon(EffectType type, Vector3 position, Transform parent)
    {
        IEffectIconView effectIcon = _pooler.Pull<IEffectIconView>(type, position, Quaternion.identity, parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IEffectIconView myPoolable)
    {
        _pooler.Push(type, myPoolable);
    }
}
