using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
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
            IsOnTurnStart = true;
        }

        public override void UseEffect(ModifiableStats stats)
        {
            Duration--;
            stats.Damage.Value *= 2;
        }
    }
}

