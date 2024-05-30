using Interactions;

namespace Abilities
{
    public interface IAbility
    {
        public bool ReadyForUse { get; }
        public int ReloadTime { get; }
        public int TurnsLeftToReload { get; }
        public IInteraction Interaction { get; }
        public AbilityType Type { get; }
        public ProjectileType ProjectileType { get; }
        public void UseAbility();
        public void TickReload();
        public void SetForReload();
        public int GetUsefulness();
        public float GetLaunchModifier();

    }
}
