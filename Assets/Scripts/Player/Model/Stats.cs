
using System;

namespace Player
{
    [Serializable]
    public class Stats
    {
        private int _speed;
        private int _health;
        private int _damage;
        public int Speed {get { return _speed;}} 
        public int Health { get { return _health;}}
        public int Damage { get { return _damage;}}

        public Stats(int speed, int health, int damage)
        {
            _damage = damage;
            _speed = speed;
            _health = health;
        }
    }
}
