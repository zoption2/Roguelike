using System;

namespace CharactersStats
{
    public class ReactiveStats
    {
        public ReactiveInt Speed = new();
        public ReactiveInt Health = new();
        public ReactiveInt Damage = new();
        public ReactiveFloat LaunchPower = new();
        public ReactiveFloat Velocity = new();

        //private ReactiveInt _speed;
        //private ReactiveInt _health;
        //private ReactiveInt _damage;
        //private ReactiveFloat _launchPower;
        //private ReactiveFloat _velocity;

        //public int Speed { get { return _speed.Value; } set { _speed.Value = value; } }
        //public int Health { get { return _health.Value; } set { _health.Value = value; } }
        //public int Damage { get { return _damage.Value; } set { _damage.Value = value; } }
        //public float LaunchPower { get { return _launchPower.Value; } set { _launchPower.Value = value; } }
        //public float Velocity { get { return _velocity.Value; } set { _velocity.Value = value; } }

        //public ModifiableStats(OriginStats originStats = null)
        //{
        //    if (originStats != null)
        //    {
        //        _speed.Value = originStats.Speed;
        //        _health.Value = originStats.Health;
        //        _damage.Value = originStats.Damage;
        //        _launchPower.Value = originStats.LaunchPower;
        //        _velocity.Value = originStats.Velocity;
        //    }
        //    else
        //    {
        //        _speed = new ReactiveInt(0);
        //        _health = new ReactiveInt(0);
        //        _damage = new ReactiveInt(0);
        //        _launchPower = new ReactiveFloat(0f);
        //        _velocity = new ReactiveFloat(0f);
        //    }
        //}
    }

    //public class ReactiveStats
    //{
    //    public ReactiveInt Speed = new();
    //    public ReactiveInt Health = new();
    //    public ReactiveInt Damage = new();
    //    public ReactiveFloat LaunchPower = new();
    //    public ReactiveFloat Velocity = new();

    //}
}
