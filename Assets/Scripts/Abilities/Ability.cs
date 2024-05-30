using Interactions;

namespace Abilities
{
    public abstract class Ability : IAbility
    {
        public int ReloadTime { get; }
        public IInteraction Interaction { get; }
        public bool ReadyForUse { get; private set; }
        public int TurnsLeftToReload { get; private set; }
        public AbilityType Type { get; }
        public ProjectileType ProjectileType { get; protected set; }

        protected float _launchModifier;

        protected Ability(IInteraction interaction, int reloadTime, float launchMod, AbilityType type)
        {
            ReadyForUse = true;
            TurnsLeftToReload = 0;
            Interaction = interaction;
            ReloadTime = reloadTime;
            _launchModifier = launchMod;
            Type = type;
            ProjectileType = ProjectileType.None;
        }

        public void TickReload()
        {
            if (TurnsLeftToReload > 0)
            {
                TurnsLeftToReload--;
            }
            if (TurnsLeftToReload == 0 && ReadyForUse == false)
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
            //For now we will think that the more damage skill gives the more useful it is
            return Interaction.GetDamage();
        }

        public void UseAbility()
        {
            // do something and then set for reload
            SetForReload();
        }

        public float GetLaunchModifier()
        {
            return _launchModifier;
        }
    }
}
