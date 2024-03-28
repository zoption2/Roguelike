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
        ReactiveStats ProcessInteraction(IInteraction interaction);
        //void GetInteractionResult(out ReactiveStats result, Action<ReactiveStats> callback);
        //void SetToDefault();
    }

    public class InteractionProcessor : IInteractionProcessor
    {
        private IEffectProcessor _effector;

        public void Init(IEffectProcessor effector)
        {
            _effector = effector;
        }

        
        public ReactiveStats ProcessInteraction(IInteraction interaction)
        {
            var stats = new ReactiveStats();

            _effector.ProcessStatsBeforeInteraction(stats);

            interaction.Interacte(stats);

            _effector.ProcessEffectsOnEnd(stats);

            //UpdateModifiableStats(stats);
            Debug.LogWarning("Interaction was handled!");

            return stats;
        }

        //private void UpdateModifiableStats(ReactiveStats stats)
        //{
        //    _interationHandlerInteractionResult = stats;
        //}

        //public void GetInteractionResult(out ReactiveStats result, Action<ReactiveStats> callback)
        //{
        //    result = _interationHandlerInteractionResult;
        //    callback?.Invoke(_interationHandlerInteractionResult);
        //}

        //public void SetToDefault()
        //{
        //    _interationHandlerInteractionResult = new();
        //}

    }

}


