using CharactersStats;

namespace Interactions
{
    public class ArrowWithoutBounceAttack : InteractionBase
    {
        public ArrowWithoutBounceAttack(int damage) : base(damage)
        {
            _damageMultiplayer = 1;

            _effects = new()
            {
            };

            _movable = new AbsentBounce();
        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }

    }
}
