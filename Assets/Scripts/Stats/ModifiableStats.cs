using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CharactersStats
{
    public class ModifiableStats
    {
        private int _speed;
        private int _health;
        private int _damage;
        private float _launchPower;

        public int Speed { get { return _speed; } set { _speed = value; } }
        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public float LaunchPower { get { return _launchPower; } set { _launchPower = value; } }

        public ModifiableStats(OriginStats originStats)
        {
            _speed = originStats.Speed;
            _health = originStats.Health;
            _damage = originStats.Damage;
            _launchPower = originStats.LaunchPower;
        }
    }
}

