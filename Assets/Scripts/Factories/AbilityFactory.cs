using CharactersStats;
using Interactions;

namespace Abilities
{
    public interface IAbilityFactory
    {
        IAbility CreateAbility(AbilityType type, ReactiveStats stats);
    }

    public class AbilityFactory : IAbilityFactory
    {
        private IInteractionFactory _interactionFactory;
        private AbilityHolder _abilityHolder;
        public AbilityFactory(IInteractionFactory interactionFactory, AbilityHolder abilityHolder)
        {
            _interactionFactory = interactionFactory;
            _abilityHolder = abilityHolder;
        }
        public IAbility CreateAbility(AbilityType type, ReactiveStats stats)
        {
            AbilityMapper mapper = _abilityHolder.GetAbilityData(type);
            IInteraction interaction = _interactionFactory.Create(mapper.InteractionType, stats);
            switch (type)
            {
                case AbilityType.HeavyAttackAbility:
                    return new KnightAttackAbility(interaction, mapper.ReloadTime, mapper.LaunchModifier, type);
                case AbilityType.MegaHeavyAttackAbility:
                    return new MegaHeavyAttackAbility(interaction, mapper.ReloadTime, mapper.LaunchModifier, type);
                case AbilityType.ArrowShootAbility:
                    return new ArrowShootAbility(interaction, mapper.ReloadTime, mapper.LaunchModifier, type);
                default:
                    return new BasicAttackAbility(interaction, mapper.ReloadTime, mapper.LaunchModifier, type);
            }
        }
    }
}
