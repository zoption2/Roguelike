using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerModel
    {
        private float _power;

        private float GetPower()
        {
            return _power;
        }

        private void SetPower(float power)
        {
            _power = power;
        }
    }
}

