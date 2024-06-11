using Interactions;

namespace Abilities
{
    public abstract class Ability : IAbility
    {
        public int ReloadTime { get; }
        public int RicochetCount { get; }
        public IInteraction Interaction { get; }
        public bool ReadyForUse { get; private set; }
        public int TurnsLeftToReload { get; private set; }
        public AbilityType Type { get; }
        public ProjectileType ProjectileType { get; protected set; }

        protected float _launchModifier;

        protected Ability(IInteraction interaction, int reloadTime, float launchMod, AbilityType type,ProjectileType projectileType, int ricochetCount)
        {
            ReadyForUse = true;
            TurnsLeftToReload = 0;
            Interaction = interaction;
            ReloadTime = reloadTime;
            _launchModifier = launchMod;
            Type = type;
            ProjectileType = projectileType;
            RicochetCount = ricochetCount;
        }

        public void TickReload()
        {
            if (TurnsLeftToReload > 0)
            {
                TurnsLeftToReload--;
            }
            else if (TurnsLeftToReload == 0 && ReadyForUse == false)
            {
                ReadyForUse = true;
            }
        }

        public void SetForReload()
        {
            TurnsLeftToReload = ReloadTime;
            if (TurnsLeftToReload > 0)
            {
                ReadyForUse = false;
            }
        }

        public int GetUsefulness()
        {
            return Interaction.GetDamage();
        }

        public void UseAbility()
        {
            SetForReload();
        }

        public float GetLaunchModifier()
        {
            return _launchModifier;
        }
    }
}
