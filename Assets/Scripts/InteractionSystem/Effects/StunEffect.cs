using CharactersStats;

namespace Interactions
{
    public class StunEffect : EffectBase
    {
        public StunEffect(int duration)
        {
            Duration = duration;
            IsOnInteractionStart = false;
            IsPositive = false;
            IsOnTurnStart = true;
            IsOnTurnEnd = false;
        }

        public override void UseEffect(ReactiveStats stats)
        {
            Duration--;
        }
    }
}