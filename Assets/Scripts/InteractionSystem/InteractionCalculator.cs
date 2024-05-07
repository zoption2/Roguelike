using CharactersStats;
using UnityEngine;

namespace Interactions
{
    public interface IInteractionCalculator
    {
        ReactiveStats CalculateInteractionResult(ReactiveStats modifiableStats, ReactiveStats interactionResult);
    }

    public class InteractionCalculator : IInteractionCalculator
    {
        public ReactiveStats CalculateInteractionResult(ReactiveStats modifiableStats, ReactiveStats interactionResult)
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


