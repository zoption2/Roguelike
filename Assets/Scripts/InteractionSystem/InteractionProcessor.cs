using CharactersStats;

namespace Interactions
{
    public interface IInteractionProcessor
    {
        ReactiveStats ProcessInteraction(IInteraction interaction);
    }

    public class InteractionProcessor : IInteractionProcessor
    {        
        public ReactiveStats ProcessInteraction(IInteraction interaction)
        {
            var stats = new ReactiveStats();

            interaction.Interact(stats);

            return stats;
        }
    }

}


