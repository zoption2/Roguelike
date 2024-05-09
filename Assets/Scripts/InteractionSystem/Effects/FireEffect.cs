using CharactersStats;

namespace Interactions
{
    public class FireEffect : EffectBase
    {
        public FireEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = false;
            IsPositive = false;
            IsOnTurnStart = true;
            IsOnTurnEnd = false;
            _effectType = EffectType.FireEffect;
        }

        public override void UseEffect(ReactiveStats stats)
        {
            Duration--;
            stats.Health.Value -= 2;
        }
    }
}
