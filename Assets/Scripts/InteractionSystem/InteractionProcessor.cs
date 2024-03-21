using CharactersStats;
using Prefab;
using UnityEngine;
using Zenject;

namespace Interactions
{
    public interface IInteractionProcessor
    {
        void UseInteractionProcessor(Stats dealerStats, IEffector effector);
        void HandleInteraction();
        void GetInteractionHandler(IInteractible interactible);
        void GetInteraction(InteractionType type);
    }

    public class InteractionProcessor : IInteractionProcessor
    {
        private Stats _interationHandlerStatsCopy;
        private Stats _interationDealerStatsCopy;
        private IEffector _effector;
        private IInteractible _interactible;
        private IInteraction _interaction;

        [Inject]
        private IInteractionFactory _interactionFactory;

        public void UseInteractionProcessor(Stats stats, IEffector effector)
        {
            _interaction = null;
            _interationDealerStatsCopy = stats;
            _effector = effector;
        }

        public void GetInteractionHandler(IInteractible interactible)
        {
            _interactible = interactible;
        }

        public void GetInteraction(InteractionType type)
        {
            IInteraction interaction = _interactionFactory.Create(type, _interationDealerStatsCopy);
            _interaction = interaction;
        }
        
        public void HandleInteraction() {

            CheckPreAttackEffects();
            _interaction.Interacte(_interationDealerStatsCopy);
            CheckPostAttackEffects();

            _interactible.GetStatsAfterInteraction(_interationHandlerStatsCopy);
            Debug.LogWarning("Interaction was handled!");
        }

        private void CheckPreAttackEffects()
        {
            foreach(IEffect effect in _effector.GetPreInteractionEffects())
            {
                if (_effector.GetPreInteractionEffects().Count <= 0) return;
                _interationHandlerStatsCopy = effect.UseEffect();
            }
        }

        private void CheckPostAttackEffects()
        {
            foreach (IEffect effect in _effector.GetPostInteractionEffects())
            {
                if (_effector.GetPostInteractionEffects().Count <= 0) return;
                _interationHandlerStatsCopy = effect.UseEffect();
            }
        }

        public Stats UpdateStats(Stats stats)
        {
            ///////
            return stats;
        }
    }

}


