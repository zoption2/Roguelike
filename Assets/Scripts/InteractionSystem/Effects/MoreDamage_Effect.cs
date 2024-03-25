using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class MoreDamage_Effect : EffectBase, IPreInteractionEffect
    {
        public MoreDamage_Effect(int duration)
        {
            Duration = duration;
        }

        public override void UseEffect(ModifiableStats stats)
        {
            Duration--;
            stats.Damage.Value *= 2;
        }
    }
}

