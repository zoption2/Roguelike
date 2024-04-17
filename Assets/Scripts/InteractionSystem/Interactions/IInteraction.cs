using CharactersStats;
using System.Collections.Generic;

namespace Interactions
{
    public interface IInteraction
    {
        ReactiveStats Interacte(ReactiveStats stat);
        List<IEffect> GetEffects();
        IMovable GetBump();
    }
}

