using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        public int Duration { get; set; }
        protected Stats _stats;

        public EffectBase(Stats stats)
        {
            _stats = stats;
        }

        public abstract Stats UseEffect();
    }
}


