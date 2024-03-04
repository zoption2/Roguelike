using CharactersStats;
using Enemy;
using Pool;
using Prefab;
using Zenject;

public interface IEnemyFactory : ICharacterFactory<IEnemyController, EnemyType> { }
public class EnemyFactory : CharacterFactory<IEnemyController, EnemyType>, IEnemyFactory
{
    private EnemyPooler _concretePooler;
    protected override ObjectPooler<EnemyType> _pooler { get => _concretePooler; }

    public EnemyFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        EnemyPooler enemyPooler)
        : base(container, statsProvider)
    {
        _concretePooler = enemyPooler;
        _concretePooler.Init();
    }
    protected override Stats GetStats(EnemyType enemyType, int id)
    {
        return _statsProvider.GetCharacterStats(enemyType, id);
    }
}

