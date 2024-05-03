using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAbilityFactory
{
    IAbility CreateAbility(AbilityType type, ReactiveStats stats);
}

public class AbilityFactory : IAbilityFactory
{
    private IInteractionFactory _interactionFactory;
    public AbilityFactory(IInteractionFactory interactionFactory)
    {
        _interactionFactory = interactionFactory;
    }
    public IAbility CreateAbility(AbilityType type, ReactiveStats stats)
    {
        InteractionType interactionType;
        IInteraction interaction;
        switch (type)
        {
            case AbilityType.HeavyAttackAbility:
                interactionType = InteractionType.Knight_HeavyAttack;
                interaction = _interactionFactory.Create(interactionType, stats);
                return new KnightAttackAbility(interaction, 2, 1f);
            case AbilityType.MegaHeavyAttackAbility:
                interactionType = InteractionType.SuperMegaHeavyAttack;
                interaction = _interactionFactory.Create(interactionType, stats);
                return new MegaHeavyAttackAbility(interaction, 5, 0.5f);
            default:
                interactionType = InteractionType.BasicAttack;
                interaction = _interactionFactory.Create(interactionType, stats);
                return new BasicAttackAbility(interaction, 0, 1f);
        }
    }
}
