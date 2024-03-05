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
        private float _velocity;
        public int Speed {get { return _speed;}} 
        public int Health { get { return _health;}}
        public int Damage { get { return _damage;}}
        public float Velocity { get { return _velocity; } }

        public Stats(int speed, int health, int damage, float velocity)
        {
            _damage = damage;
            _speed = speed;
            _health = health;
            _velocity = velocity;
        }
    }
}
