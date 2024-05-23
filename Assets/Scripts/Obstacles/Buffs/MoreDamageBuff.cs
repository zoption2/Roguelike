using Interactions;
using Obstacles;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoreDamageBuff : MonoBehaviour, IBuff, IMyPoolable
{
    [SerializeField] private BuffType _type;

    [SerializeField] private float _probability;

    private BuffPooler _pooler;

    public void Init(BuffPooler pooler)
    {
        _pooler = pooler;
        _pooler.Init();
    }

    public List<IEffect> UseBuff()
    {
        _pooler.Init();
        List<IEffect> effects = new List<IEffect>()
        {
            new MoreDamageEffect(1),
        };

        return effects;
    }

    public void RemoveBuff()
    {
        _pooler.Push(_type, this);
    }

    public void OnCreate()
    {
    }

    public void OnPull()
    {
    }

    public void OnRelease()
    {
    }

    public BuffType GetBuffType()
    {
        return _type;
    }

    public float GetBuffProbability()
    {
        return _probability;
    }
}
