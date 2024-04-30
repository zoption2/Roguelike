using CharactersStats;
using Enemy;
using Interactions;
using Pool;
using Prefab;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, CharacterType type);
}
public class EnemyFactory : CharacterFactory<IEnemyController>, IEnemyFactory
{
    public EnemyFactory(DiContainer container, IStatsProvider statsProvider, CharacterPooler pooler, 
        IAbilityFactory abilityFactory) : base(container, statsProvider, pooler, abilityFactory)
    {
        
    }

    protected override OriginStats GetStats(CharacterType type)
    {
        return _statsProvider.GetEnemyStats(type);
    }

    protected override List<AbilityType> GetAbilitiesTypes(CharacterType type)
    {
        return _statsProvider.GetCharacterAbilitiesTypes(type);
    }

    public IEnemyController CreateEnemy(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}