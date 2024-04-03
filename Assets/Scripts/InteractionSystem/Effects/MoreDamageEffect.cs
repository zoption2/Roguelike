using CharactersStats;
using UnityEngine;

namespace Interactions
{
    public class MoreDamageEffect : EffectBase
    {
        public MoreDamageEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = true;
            IsPositive = true;
            IsOnTurnStart = false;
            IsOnTurnEnd = false;
        }

        public override void UseEffect(ReactiveStats stats)
        {
            Duration--;
            stats.Damage.Value *= 2;
        }
    }
}

