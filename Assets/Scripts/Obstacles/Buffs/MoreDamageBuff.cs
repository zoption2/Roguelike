using Interactions;
using Obstacles;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MoreDamageBuff : TriggerBase, IBuff
{
    [SerializeField] private TriggerType _type;

    [Inject]
    private TriggerPooler _pooler;

    private void Start()
    {
        _pooler.Init();
    }

    public List<IEffect> UseBuff()
    {
        
        List<IEffect> effects = new List<IEffect>()
        {
            new MoreDamageEffect(1),
        };

        return effects;
    }

    public void DisableTrigger()
    {
        _pooler.Push(_type, this);
    }
}
