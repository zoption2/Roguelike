using CharactersStats;

namespace Interactions
{
    public interface IEffect
    {
        void UseEffect(ReactiveStats stats);
        EffectType GetEffectType();
        int Duration { get; set; }
        bool IsPositive { get; set; }
        bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        public bool IsOnTurnEnd { get; set; }
    }
}

