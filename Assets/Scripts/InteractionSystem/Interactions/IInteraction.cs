using CharactersStats;
using System.Collections.Generic;

namespace Interactions
{
    public interface IInteraction
    {
        public bool CouldUseAbility();
        public int GetDamage();
        public void TickReload();
        public float GetLaunchMultiplier();

        public TypeOfAttack GetAttackType();
        public ReactiveStats Interact(ReactiveStats stats);
        ReactiveStats InteractWithStats(ReactiveStats stats);
        List<IEffect> GetEffects();
        public void SetStats(ReactiveStats stats);
        IMovable GetBump();
    }
}

