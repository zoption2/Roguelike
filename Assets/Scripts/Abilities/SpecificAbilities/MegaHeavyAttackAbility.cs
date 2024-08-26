using Interactions;

namespace Abilities
{
    public class MegaHeavyAttackAbility : Ability
    {
        public MegaHeavyAttackAbility(IInteraction interaction, int reloadTime, float launchMod, AbilityType type,
            ProjectileType projectileType, int ricochetCount)
            : base(interaction, reloadTime, launchMod, type, projectileType, ricochetCount)
        {
        }
    }
}
