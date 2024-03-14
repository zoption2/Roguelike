using CharactersStats;
using Enemy;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, CharacterType type);
}
public class EnemyFactory : CharacterFactory<IEnemyController, EnemyPooler>, IEnemyFactory
{
    private EnemyPooler _pooler;
    public EnemyFactory(DiContainer container, IStatsProvider statsProvider, EnemyPooler pooler) : base(container, statsProvider, pooler)
    {
        _pooler = pooler;
        _characterPooler.Init();
    }

    protected override Stats GetStats(CharacterType type)
    {
        return _statsProvider.GetEnemyStats(type);
    }

    protected override EnemyPooler GetPooler()
    {
        return _pooler;
    }

    public IEnemyController CreateEnemy(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }

    //public IEnemyController CreateCharacter(Transform point, EnemyType type)
    //{
    //    var characterType = (CharacterType)type;
    //    return CreateCharacter(point, characterType);
    //}
}