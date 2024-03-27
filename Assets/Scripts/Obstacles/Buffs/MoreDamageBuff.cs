using Interactions;
using Obstacles;
using Pool;
using Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoreDamageObstacle : MonoBehaviour, IBuff, IMyPoolable
{
    public BuffType Type = BuffType.MoreDamage;
    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }

    [Inject]
    private BuffPooler _pooler;

    public List<IEffect> ProcessTrigger()
    {
        _pooler.Init();
        List<IEffect> effects = new List<IEffect>()
        {
            new MoreDamageEffect(1),
        };

        return effects;
    }

    public void DisableBuff()
    {
        _pooler.Push(Type, this);
    }
}
