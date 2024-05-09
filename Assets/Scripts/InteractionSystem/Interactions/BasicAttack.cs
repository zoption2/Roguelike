using CharactersStats;

namespace Interactions 
{
    public class BasicAttack : InteractionBase
    {
        public BasicAttack(int damage) : base(damage)
        {
            _damageMultiplayer = 1;
            _effects = new()
            {
            };

        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

