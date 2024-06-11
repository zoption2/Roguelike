using CharactersStats;
using System;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        protected EffectType _effectType;
        protected int _duration;

        public int Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnDurationChanged(); 
                }
            }
        }

        public bool IsPositive { get; set; }
        public bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        public bool IsOnTurnEnd { get; set; }
        public abstract void UseEffect(ReactiveStats stats);
        public EffectType GetEffectType()
        {
            return _effectType;
        }

        public event Action ON_DURATION_CHANGED;

        private void OnDurationChanged()
        {
            ON_DURATION_CHANGED?.Invoke();
        }

        public abstract EffectBase Clone();
    }
}
