using CharactersStats;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Interactions
{
    public interface IInteractionDealer
    {
        public void Init(Stats stats);
        void StartInteractionProcess(InteractionType type);
        void AddInteraction(InteractionType type);

        Queue<IInteraction> GetQueue();
    }

    public class InteractionDealer : IInteractionDealer
    {
        private Queue<IInteraction> _interactions = new();
        private Stats _damageDealerStatsCopy;

        [Inject]
        private IInteractionFactory _interactionFactory;

        public void Init(Stats stats)
        {
            _damageDealerStatsCopy = stats;
        }

        public void StartInteractionProcess(InteractionType type)
        {
            IInteraction interaction = _interactionFactory.Create(type, _damageDealerStatsCopy);
            if(!_interactions.Contains(interaction)) _interactions.Enqueue(interaction);
            Debug.Log($"Selected interaction {type}. Objs in Queue: {_interactions.Count}");
        }

        public void AddInteraction(InteractionType type)
        {
            IInteraction interaction = _interactionFactory.Create(type, _damageDealerStatsCopy);
            _interactions.Enqueue(interaction);
            Debug.Log($"Added interaction {type}. Objs in Queue: {_interactions.Count}");
        }

        public Queue<IInteraction> GetQueue()
        {
            return _interactions;
        }
    }



}

