using CharactersStats;
using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class MoreDamage_Effect : EffectBase, IPreInteractionEffect
    {
        public MoreDamage_Effect(Stats stats, int duration) : base(stats)
        {
            _stats = stats;
            Duration = duration;
        }

        public override Stats UseEffect()
        {
            Duration--;
            _stats.Damage *= 2;
            return _stats;
        }
    }
}

