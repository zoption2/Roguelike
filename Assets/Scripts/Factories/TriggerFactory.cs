using Obstacles;
using Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ITriggerFactory
{
    public void Init();
    public IMyPoolable CreateTrigger(Transform position, TriggerType type);
}
public class TriggerFactory : ITriggerFactory
{
    private TriggerPooler _pool;

    [Inject]
    public void Construct(
        TriggerPooler pool
        )
    {
        _pool = pool;

        _pool.Init();
    }

    public void Init()
    {
    }

    public IMyPoolable CreateTrigger(Transform point, TriggerType type)
    {
        IMyPoolable newTrigger = _pool.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);

        return newTrigger;
    }

}
