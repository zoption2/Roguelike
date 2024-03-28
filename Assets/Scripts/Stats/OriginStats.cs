using System;

namespace CharactersStats
{
    public interface IStats
    {
        public int Speed { get; set; }

    }

    [Serializable]
    public class OriginStats
    {
        private ReactiveInt _speed;
        private ReactiveInt _health;
        private ReactiveInt _damage;
        private ReactiveFloat _velocity;
        private ReactiveFloat _launchPower;

        public ReactiveInt Speed { get { return _speed; } }
        public ReactiveInt Health { get { return _health; } }
        public ReactiveInt Damage { get { return _damage; } }
        public ReactiveFloat LaunchPower { get { return _launchPower; } }
        public ReactiveFloat Velocity { get { return _velocity; } }

        public OriginStats(int speed, int health, int damage, float launchPower)
        {
            _speed = new ReactiveInt(speed);
            _health = new ReactiveInt(health);
            _damage = new ReactiveInt(damage);
            _launchPower = new ReactiveFloat(launchPower);
            _velocity = new ReactiveFloat(0f);
        }
    }
}
