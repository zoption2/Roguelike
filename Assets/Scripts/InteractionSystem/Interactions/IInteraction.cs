using CharactersStats;
using System.Collections.Generic;

namespace Interactions
{
    public interface IInteraction
    {
        public int GetDamage();
        public ReactiveStats Interact(ReactiveStats stats);
        public ReactiveStats InteractWithStats(ReactiveStats stats);
        public List<IEffect> GetEffects();
        public void SetStats(ReactiveStats stats);
        public IMovable GetBump();
    }
}

