using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactions
{
    public class PoisonEffect : EffectBase
    {
        public PoisonEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = false;
            IsPositive = false;
            IsOnTurnStart = true;
        }

        public override void UseEffect(ModifiableStats stats)
        {
            Duration--;
            stats.Health.Value -= 20;
        }
    }
}
