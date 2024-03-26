using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        public int Duration { get; set; }
        public bool IsPositive { get; set; }
        public bool IsOnInteractionStart { get; set; }
        public bool IsOnTurnStart { get; set; }
        protected ModifiableStats _stats;



        public abstract void UseEffect(ModifiableStats stats);
    }
}


