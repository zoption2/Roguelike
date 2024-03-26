using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IInteraction
    {
        ModifiableStats Interacte(ModifiableStats stat);
        IEffect GetEffect();
    }
}

