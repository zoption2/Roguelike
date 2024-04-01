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

            return stats;
        }
    }

}


