using Interactions;

namespace Abilities
{
    public class ArrowShootAbility : Ability
    {
        public ArrowShootAbility(IInteraction interaction, int reloadTime, float launchMod, AbilityType type)
            : base(interaction, reloadTime, launchMod, type)
        {
        }
    }
}
