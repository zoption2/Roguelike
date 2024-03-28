using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IInteractionFinalizer
    {
        ReactiveStats FinalizeInteraction(ReactiveStats modifiableStats, ReactiveStats interactionResult);
    }

    public class InteractionFinalizer : IInteractionFinalizer
    {
        public ReactiveStats FinalizeInteraction(ReactiveStats modifiableStats, ReactiveStats interactionResult)
        {
            modifiableStats.Speed.Value -= Mathf.Abs(interactionResult.Speed.Value);
            modifiableStats.Health.Value -= Mathf.Abs(interactionResult.Health.Value);
            modifiableStats.Damage.Value -= Mathf.Abs(interactionResult.Damage.Value);
            modifiableStats.LaunchPower.Value -= Mathf.Abs(interactionResult.LaunchPower.Value);
            modifiableStats.Velocity.Value -= Mathf.Abs(interactionResult.Velocity.Value);
            

            return modifiableStats;
        }
    }
}


