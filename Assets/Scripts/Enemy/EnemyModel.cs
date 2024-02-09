using Player;

namespace Enemy
{
    public class EnemyModel
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