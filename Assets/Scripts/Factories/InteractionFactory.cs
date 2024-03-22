using Interactions;
using Prefab;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInteractionFactory
{
    IInteraction Create(InteractionType type, CharacterModel stats);
}

public class InteractionFactory : IInteractionFactory
{
    public IInteraction Create(InteractionType type, CharacterModel stats)
    {
        switch (type)
        {
            case InteractionType.BasicAttack:
                return new BasicAttack(stats.Damage);
            case InteractionType.Knight_HeavyAttack:
                return new Knight_HeavyAttack(stats.Damage, 2);
            default:
                return new BasicAttack(stats.Damage);
        }
    }
}

