using System;

namespace CharactersStats
{
    public interface IStats
    {
        public int Speed { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public float LaunchPower { get; set; }
        public float Velocity { get; set; }

    }

    [Serializable]
    public class OriginStats : IStats
    {
        private int _speed;
        private int _health;
        private int _damage;
        private float _velocity;
        private float _launchPower;

        public int Speed { get { return _speed; } set { _speed = value; } }
        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public float LaunchPower { get { return _launchPower; } set { _launchPower = value; } }
        public float Velocity { get { return _velocity; } set { _velocity = value; } }

        public OriginStats()
        {          
        }

        public OriginStats(int speed, int health, int damage, float launchPower)
        {
            _speed = speed;
            _health = health;
            _damage = damage;
            _launchPower = launchPower;
            _velocity = 0f;
        }
    }

    public static class StatsExtension
    {
        public static ReactiveStats ToReactive(this OriginStats origin)
        {
            var modifiable = new ReactiveStats();
            modifiable.Speed = new ReactiveInt(origin.Speed);
            modifiable.Health = new ReactiveInt(origin.Health);
            modifiable.Damage = new ReactiveInt(origin.Damage);
            modifiable.LaunchPower = new ReactiveFloat(origin.LaunchPower);
            modifiable.Velocity = new ReactiveFloat(origin.Velocity);

            return modifiable;
        }

        public static OriginStats ToOrigin(this ReactiveStats reactive)
        {
            var origin = new OriginStats();
            origin.Speed = reactive.Speed.Value;
            origin.Health = reactive.Health.Value;
            origin.Damage = reactive.Damage.Value;
            origin.LaunchPower = reactive.LaunchPower.Value;
            origin.Velocity = reactive.Velocity.Value;

            return origin;
        }
    }
}
