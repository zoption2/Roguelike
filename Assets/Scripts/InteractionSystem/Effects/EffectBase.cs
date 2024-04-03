using CharactersStats;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        public int Duration { get; set; }
        public bool IsPositive { get; set; }
        public bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        public bool IsOnTurnEnd { get; set; }
        public abstract void UseEffect(ReactiveStats stats);
    }
}


