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

        public ModifiableStats(OriginStats originStats = null)
        {
            if (originStats != null)
            {
                _speed = originStats.Speed;
                _health = originStats.Health;
                _damage = originStats.Damage;
                _launchPower = originStats.LaunchPower;
                _velocity = originStats.Velocity;
            }
            else
            {
                _speed = new ReactiveInt(0);
                _health = new ReactiveInt(0);
                _damage = new ReactiveInt(0);
                _launchPower = new ReactiveFloat(0f);
                _velocity = new ReactiveFloat(0f);
            }
        }
    }
}
