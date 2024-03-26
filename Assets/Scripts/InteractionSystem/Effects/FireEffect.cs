using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interactions
{
    public class FireEffect : EffectBase
    {
        public FireEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = false;
            IsPositive = false;
            IsOnTurnStart = false;
        }

        public override void UseEffect(ModifiableStats stats)
        {
            Duration--;
            stats.Health.Value -= 2;
        }
    }
}
