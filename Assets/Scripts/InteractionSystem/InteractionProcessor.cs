using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IInteractionProcessor
    {
        public void Init(Stats stats);
        public void StartProcessing(Queue<IInteraction> queue);
    }

    public class InteractionProcessor : IInteractionProcessor
    {
        private Stats _interationHandlerStatsCopy;

        public void Init(Stats stats)
        {
            _interationHandlerStatsCopy = stats;
        }
        public void StartProcessing(Queue<IInteraction> queue)
        {
            foreach (var interaction in queue)
            {
                interaction.Interacte(_interationHandlerStatsCopy);
            }
        }
    }
}


