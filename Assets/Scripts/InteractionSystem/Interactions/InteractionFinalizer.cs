using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IInteractionFinalizer
    {
        void Init(IInteractionProcessor processor, IEffectProcessor effectProcessor);
        ModifiableStats FinalizeInteraction(ModifiableStats modifiableStats);
    }

    public class InteractionFinalizer : IInteractionFinalizer
    {
        private IInteractionProcessor _processor;
        private IEffectProcessor _effectProcessor;
        private ModifiableStats _interactionResult;
        private ModifiableStats _stats;
        public void Init(IInteractionProcessor processor, IEffectProcessor effectProcessor)
        {
            _processor = processor;
            _effectProcessor = effectProcessor;
        }

        public ModifiableStats FinalizeInteraction(ModifiableStats modifiableStats)
        {
            _stats = modifiableStats;

            _processor.GetInteractionResult(out _interactionResult, result =>
            {
                _interactionResult = result;
                _processor.SetToDefault();
            });

            _stats.Speed.Value -= Mathf.Abs(_interactionResult.Speed.Value);
            _stats.Health.Value -= Mathf.Abs(_interactionResult.Health.Value);
            _stats.Damage.Value -= Mathf.Abs(_interactionResult.Damage.Value);
            _stats.LaunchPower.Value -= Mathf.Abs(_interactionResult.LaunchPower.Value);
            _stats.Velocity.Value -= Mathf.Abs(_interactionResult.Velocity.Value);
            

            return _stats;
        }
    }
}


