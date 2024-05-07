using CharactersStats;

namespace Interactions
{
    public class KnightHeavyAttack : InteractionBase
    {
        public KnightHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;
            _effects = new()
            {
                new StunEffect(2),
            };

            _movable = new StopAndPush();
        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


