using CharactersStats;

namespace Interactions
{
    public class MegaHeavyAttack : InteractionBase
    {

        public MegaHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;
            _effects = new()
            {
               new FireEffect(3)
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
