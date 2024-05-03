using CharactersStats;

public interface ICharacterModel
{
}
public abstract class CharacterModelBase : ICharacterModel
{
    protected float _velocity;
    protected int _damage;
    protected int _health;
    protected int _speed;
    protected float _launchPower;
    protected OriginStats _originStats;
    protected ReactiveStats _reactiveStats;

    public CharacterModelBase(OriginStats originStats)
    {
        _damage = originStats.Damage;
        _health = originStats.Health;
        _speed = originStats.Speed;
        _launchPower = originStats.LaunchPower;
        _velocity = originStats.Velocity;
        _originStats = originStats;
        _reactiveStats = _originStats.ToReactive();
    }

    public OriginStats GetStats()
    {
        return _originStats;
    }

    public ReactiveStats GetReactiveStats()
    {
        return _reactiveStats;
    }


}
