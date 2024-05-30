using Interactions;

namespace Abilities
{
    public class BasicAttackAbility : Ability
    {
        public BasicAttackAbility(IInteraction interaction, int reloadTime, float launchMod, AbilityType type)
            : base(interaction, reloadTime, launchMod, type)
        {

        }
    }
}
