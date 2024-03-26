using CharactersStats;
using Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Interactions
{
    public interface IInteractionProcessor
    {
        void Init(IEffectProcessor effector);
        void ProcessInteraction(IInteraction interaction);
        void GetInteractionResult(out ModifiableStats result, Action<ModifiableStats> callback);
        void SetToDefault();
    }

    public class InteractionProcessor : IInteractionProcessor
    {
        private ModifiableStats _interationHandlerInteractionResult;
        private IEffectProcessor _effector;

        public void Init(IEffectProcessor effector)
        {
            _effector = effector;
            _interationHandlerInteractionResult = new();
        }

        
        public void ProcessInteraction(IInteraction interaction)
        {
            _interationHandlerInteractionResult = new();

            _effector.ProcessStatsBeforeInteraction(_interationHandlerInteractionResult);

            interaction.Interacte(_interationHandlerInteractionResult);

            _effector.ProcessEffectsOnEnd(_interationHandlerInteractionResult);

            UpdateModifiableStats(_interationHandlerInteractionResult);
            Debug.LogWarning("Interaction was handled!");
        }

        private void UpdateModifiableStats(ModifiableStats stats)
        {
            _interationHandlerInteractionResult = stats;
        }

        public void GetInteractionResult(out ModifiableStats result, Action<ModifiableStats> callback)
        {
            result = _interationHandlerInteractionResult;
            callback?.Invoke(_interationHandlerInteractionResult);
        }

        public void SetToDefault()
        {
            _interationHandlerInteractionResult = new();
        }

    }

}


