using CharactersStats;
using System;

namespace Interactions
{
    public interface IEffect
    {
        public event Action ON_DURATION_CHANGED;
        void UseEffect(ReactiveStats stats);
        EffectType GetEffectType();
        int Duration { get; set; }
        bool IsPositive { get; set; }
        bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        public bool IsOnTurnEnd { get; set; }
    }
}

