using CharactersStats;
using Prefab;
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

