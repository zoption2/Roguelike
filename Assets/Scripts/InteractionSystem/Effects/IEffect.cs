using System.Collections;
using System.Collections.Generic;
using CharactersStats;
using UnityEngine;

namespace Interactions
{
    public interface IEffect
    {
        Stats UseEffect();
        int Duration { get; set; }
    }

    public interface IPreInteractionEffect { }
    public interface IPostInteractionEffect { }
}

