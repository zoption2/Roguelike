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
        public void Init(CharacterModel stats);
        void UseInteraction(InteractionType type);
        void AddInteraction(InteractionType type);
        Queue<IInteraction> GetQueue();
    }

    public class InteractionDealer : IInteractionDealer
    {
        private Queue<IInteraction> _interactions;
        private CharacterModel _damageDealerStatsCopy;

        [Inject]
        private IInteractionFactory _interactionFactory;

        public void Init(CharacterModel stats)
        {
            _damageDealerStatsCopy = stats;
            _interactions = new Queue<IInteraction>();
        }

        public void UseInteraction(InteractionType type)
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

