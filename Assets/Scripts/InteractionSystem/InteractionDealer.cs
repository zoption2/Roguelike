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
        public void Init(ReactiveStats stats);
        IInteraction UseInteraction(InteractionType type);
    }

    public class InteractionDealer : IInteractionDealer
    {
        private IInteraction _interaction;
        private ReactiveStats _damageDealerStatsCopy;

        [Inject]
        private IInteractionFactory _interactionFactory;

        public void Init(ReactiveStats stats)
        {
            _damageDealerStatsCopy = stats;
        }

        public IInteraction UseInteraction(InteractionType type)
        {
            IInteraction interaction = _interactionFactory.Create(type, _damageDealerStatsCopy);
            return interaction;
        }
    }



}

