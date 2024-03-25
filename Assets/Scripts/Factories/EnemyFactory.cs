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
public class EnemyFactory : CharacterFactory<IEnemyController>, IEnemyFactory
{
    public EnemyFactory(DiContainer container, IStatsProvider statsProvider, CharacterPooler pooler) : base(container, statsProvider, pooler)
    {
        
    }

    protected override OriginStats GetStats(CharacterType type)
    {
        return _statsProvider.GetEnemyStats(type);
    }

    public IEnemyController CreateEnemy(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}