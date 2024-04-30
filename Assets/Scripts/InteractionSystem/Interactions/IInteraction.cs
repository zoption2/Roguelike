using CharactersStats;
using System.Collections.Generic;

namespace Interactions
{
    public interface IInteraction
    {
        public int GetDamage();
        public ReactiveStats Interact(ReactiveStats stats);
        ReactiveStats InteractWithStats(ReactiveStats stats);
        List<IEffect> GetEffects();
        public void SetStats(ReactiveStats stats);
        IMovable GetBump();
    }
}

