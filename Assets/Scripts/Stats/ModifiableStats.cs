using System;

namespace CharactersStats
{
    public class ModifiableStats
    {
        private ReactiveInt _speed;
        private ReactiveInt _health;
        private ReactiveInt _damage;
        private ReactiveFloat _launchPower;
        private ReactiveFloat _velocity;

        public ReactiveInt Speed { get { return _speed; } }
        public ReactiveInt Health { get { return _health; } }
        public ReactiveInt Damage { get { return _damage; } }
        public ReactiveFloat LaunchPower { get { return _launchPower; } }
        public ReactiveFloat Velocity { get { return _velocity; } }

        public ModifiableStats(OriginStats originStats)
        {
            _speed = originStats.Speed;
            _health = originStats.Health;
            _damage = originStats.Damage;
            _launchPower = originStats.LaunchPower;
            _velocity = originStats.Velocity;
        }
    }
}
