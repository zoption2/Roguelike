using Player;
using System;

namespace CharactersStats
{
    [Serializable]
    public class OriginStats
    {
        private int _speed;
        private int _health;
        private int _damage;
        private float _launchPower;
        public int Speed { get { return _speed; } set { _speed = value; } } 
        public int Health { get { return _health;} set {_health = value;} }
        public int Damage { get { return _damage;} set { _damage = value; } }
        public float LaunchPower { get { return _launchPower; } set { _launchPower = value; } }

        public OriginStats(int speed, int health, int damage, float launchPower)
        {
            _damage = damage;
            _speed = speed;
            _health = health;
            _launchPower = launchPower;
        }
    }
}
