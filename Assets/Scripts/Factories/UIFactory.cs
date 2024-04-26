using Pool;
using UnityEngine;
using Zenject;

public interface IUIFactory
{
    public IMyPoolable CreateEffectIcon(EffectType type, Vector3 position, Transform parent);
    public void RemoveEffectIcon(EffectType type, IMyPoolable myPoolable);
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

    public IMyPoolable CreateEffectIcon(EffectType type, Vector3 position, Transform parent)
    {
        IMyPoolable effectIcon = _pooler.Pull<IMyPoolable>(type, position, Quaternion.identity, parent);
        return effectIcon;
    }

    public void RemoveEffectIcon(EffectType type, IMyPoolable myPoolable)
    {
        _pooler.Push(type, myPoolable);
    }
}
