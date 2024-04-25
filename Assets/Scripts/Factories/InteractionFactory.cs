using Interactions;
using Prefab;
using CharactersStats;

public interface IInteractionFactory
{
    IInteraction Create(InteractionType type, ReactiveStats stats);
}

public class InteractionFactory : IInteractionFactory
{
    public IInteraction Create(InteractionType type, ReactiveStats stats)
    {
        switch (type)
        {
            case InteractionType.BasicAttack:
                return new BasicAttack(stats.Damage.Value);
            case InteractionType.Knight_HeavyAttack:
                return new KnightHeavyAttack(stats.Damage.Value, 2);
            default:
                return new EmptyAttack(0);
        }
    }
}

