using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        public int Duration { get; set; }
        protected ModifiableStats _stats;
        bool _isPositive;
        bool _isOnStart;

        

        public abstract void UseEffect(ModifiableStats stats);
    }
}


