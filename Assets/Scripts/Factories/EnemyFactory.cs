using CharactersStats;
using Enemy;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Abilities;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Vector3 position, Transform parent, CharacterType type);
}
public class EnemyFactory : CharacterFactory<IEnemyController>, IEnemyFactory
{
    public EnemyFactory(DiContainer container, IStatsProvider statsProvider, CharacterUIPooler characterUIPooler, CharacterPooler pooler, IAbilityFactory abilityFactory) : base(container, statsProvider, characterUIPooler, pooler, abilityFactory)
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

    public IEnemyController CreateEnemy(Vector3 position, Transform parent, CharacterType type)
    {
        return base.CreateCharacter(position, parent, type);
    }
}