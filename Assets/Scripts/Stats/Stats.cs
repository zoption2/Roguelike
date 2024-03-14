using Player;
using System;

namespace CharactersStats
{
    [Serializable]
    public class Stats
    {
        public int Speed;
        public int Health;
        public int Damage;
        public float LaunchPower;

        public Stats(int speed, int health, int damage, float launchPower)
        {
            Damage = damage;
            Speed = speed;
            Health = health;
            LaunchPower = launchPower;
        }
    }
}
