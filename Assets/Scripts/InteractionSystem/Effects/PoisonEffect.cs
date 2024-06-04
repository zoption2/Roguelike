using CharactersStats;

namespace Interactions
{
    public class PoisonEffect : EffectBase
    {
        public PoisonEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = false;
            IsPositive = false;
            IsOnTurnStart = true;
        }

        public override void UseEffect(ReactiveStats stats)
        {
            Duration--;
            stats.Health.Value -= 20;
        }

        public override EffectBase Clone()
        {
            return new PoisonEffect(this.Duration);
        }
    }
}
