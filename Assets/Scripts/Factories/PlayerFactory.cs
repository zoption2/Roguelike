using CharactersStats;
using Player;
using Pool;
using Prefab;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, CharacterType type);
}

public class PlayerFactory : CharacterFactory<IPlayerController>, IPlayerFactory
{
    public PlayerFactory(DiContainer container, IStatsProvider statsProvider, CharacterPooler pooler, 
        IAbilityFactory abilityFactory) : base(container, statsProvider, pooler, abilityFactory)
    {
    }

    protected override OriginStats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }

    protected override List<AbilityType> GetAbilitiesTypes(CharacterType type)
    {
        return _statsProvider.GetCharacterAbilitiesTypes(type);
    }

    public IPlayerController CreatePlayer(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}