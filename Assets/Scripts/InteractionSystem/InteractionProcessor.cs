using CharactersStats;
using Prefab;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Interactions
{
    public interface IInteractionProcessor
    {
        void Init(ModifiableStats dealerStats, IEffector effector);
        void HandleInteraction(IInteraction interaction);
        //void GetInteractionHandler(IInteractible interactible);
        //void GetInteraction(InteractionType type);
    }

    public class InteractionProcessor : IInteractionProcessor
    {
        private ModifiableStats _interationHandlerStatsCopy;
        private ModifiableStats _interationDealerStatsCopy;
        private IEffector _effector;

        [Inject]
        private IInteractionFactory _interactionFactory;

        public void Init(ModifiableStats stats, IEffector effector)
        {
            _interationHandlerStatsCopy = stats;
            _effector = effector;
        }

        //public void GetInteractionHandler(IInteractible interactible)
        //{
        //    _interactible = interactible;
        //}

        //public void GetInteraction(InteractionType type)
        //{
        //    IInteraction interaction = _interactionFactory.Create(type, _interationDealerStatsCopy);
        //    _interaction = interaction;
        //}
        
        public void HandleInteraction(IInteraction interaction)
        {
            CheckPreAttackEffects();

            interaction.Interacte(_interationHandlerStatsCopy); 

            CheckPostAttackEffects();
            //interactible.GetStatsAfterInteraction(_interationHandlerStatsCopy);
            Debug.LogWarning("Interaction was handled!");
        }

        private void CheckPreAttackEffects()
        {
            foreach(IEffect effect in _effector.GetPreInteractionEffects())
            {
                if (_effector.GetPreInteractionEffects().Count <= 0) return;
                effect.UseEffect(_interationHandlerStatsCopy);
            }
        }

        private void CheckPostAttackEffects()
        {
            foreach (IEffect effect in _effector.GetPostInteractionEffects())
            {
                if (_effector.GetPostInteractionEffects().Count <= 0) return;
                effect.UseEffect(_interationHandlerStatsCopy);
            }
        }

        public OriginStats UpdateStats(OriginStats stats)
        {
            ///////
            return stats;
        }
    }

}


