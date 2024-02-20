
using System;

namespace Player
{
    [Serializable]
    public class Stats
    {
        private int _speed;
        public int Speed {get { return _speed;}} 
        private int _health;
        public int Health { get { return _health;}}
        private int _damage;
        public int Damage { get { return _damage;}}

        public Stats(int speed, int health, int damage)
        {
            _damage = damage;
            _speed = speed;
            _health = health;
        }
    }
}
