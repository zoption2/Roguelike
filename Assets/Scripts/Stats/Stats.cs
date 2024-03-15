using Player;
using System;

namespace CharactersStats
{
    [Serializable]
    public class Stats
    {
        private int _speed;
        private int _health;
        private int _damage;
        private float _launchPower;
        public int Speed { get { return _speed; }} 
        public int Health { get { return _health;} set {_health = value;} }
        public int Damage { get { return _damage;}}
        public float LaunchPower { get { return _launchPower; } }

        public Stats(int speed, int health, int damage, float launchPower)
        {
            _damage = damage;
            _speed = speed;
            _health = health;
            _launchPower = launchPower;
        }
    }
}
