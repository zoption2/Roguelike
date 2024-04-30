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
        IInteractionFactory interactionFactory) : base(container, statsProvider, pooler, interactionFactory)
    {
    }

    protected override OriginStats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }

    protected override List<InteractionType> GetAbilitiesTypes(CharacterType type)
    {
        return _statsProvider.GetCharacterAbilitiesTypes(type);
    }

    public IPlayerController CreatePlayer(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}