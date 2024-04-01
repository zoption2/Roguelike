using System;

namespace CharactersStats
{
    public class ReactiveStats
    {
        public ReactiveInt Speed = new();
        public ReactiveInt Health = new();
        public ReactiveInt Damage = new();
        public ReactiveFloat LaunchPower = new();
        public ReactiveFloat Velocity = new();
    }
}
