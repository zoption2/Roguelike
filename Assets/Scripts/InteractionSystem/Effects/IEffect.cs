using System.Collections;
using System.Collections.Generic;
using CharactersStats;
using UnityEngine;

namespace Interactions
{
    public interface IEffect
    {
        void UseEffect(ModifiableStats stats);
        int Duration { get; set; }
    }

    public interface IPreInteractionEffect { }
    public interface IPostInteractionEffect { }
}

