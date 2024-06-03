using Interactions;

namespace Abilities
{
    public class BasicAttackAbility : Ability
    {
        public BasicAttackAbility(IInteraction interaction, int reloadTime, float launchMod, AbilityType type,
            ProjectileType projectileType, int ricochetCount)
            : base(interaction, reloadTime, launchMod, type, projectileType, ricochetCount)
        {

        }
    }
}
