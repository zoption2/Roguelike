using CharactersStats;
using Player;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;


public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, CharacterType type);
}

public class PlayerFactory : CharacterFactory<IPlayerController>, IPlayerFactory
{
    public PlayerFactory(DiContainer container, IStatsProvider statsProvider, CharacterPooler pooler) : base(container, statsProvider, pooler)
    {
    }

    protected override Stats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }

    public IPlayerController CreatePlayer(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}