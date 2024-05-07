using CharactersStats;
using System;

namespace Interactions
{
    public interface IEffect
    {
        public event Action ON_DURATION_CHANGED;
        public void UseEffect(ReactiveStats stats);
        public EffectType GetEffectType();
        public int Duration { get; set; }
        public bool IsPositive { get; set; }
        public bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        public bool IsOnTurnEnd { get; set; }
    }
}

