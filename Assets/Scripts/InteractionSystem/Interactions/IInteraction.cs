using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IInteraction
    {
        ReactiveStats Interacte(ReactiveStats stat);
        List<IEffect> GetEffects();
    }
}

