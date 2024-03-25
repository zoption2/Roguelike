using Interactions;
using Prefab;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInteractionFactory
{
    IInteraction Create(InteractionType type, ModifiableStats stats);
}

public class InteractionFactory : IInteractionFactory
{
    public IInteraction Create(InteractionType type, ModifiableStats stats)
    {
        switch (type)
        {
            case InteractionType.BasicAttack:
                return new BasicAttack(stats.Damage.Value);
            case InteractionType.Knight_HeavyAttack:
                return new Knight_HeavyAttack(stats.Damage.Value, 2);
            default:
                return new BasicAttack(stats.Damage.Value);
        }
    }
}

