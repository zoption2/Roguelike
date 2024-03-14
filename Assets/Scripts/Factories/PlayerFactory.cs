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

public class PlayerFactory : CharacterFactory<IPlayerController, PlayerPooler>, IPlayerFactory
{
    private PlayerPooler _pooler;
    public PlayerFactory(DiContainer container, IStatsProvider statsProvider, PlayerPooler pooler) : base(container, statsProvider, pooler)
    {
        _pooler = pooler;
        pooler.Init();
    }

    protected override Stats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }
    protected override PlayerPooler GetPooler()
    {
        return _pooler;
    }

    public IPlayerController CreatePlayer(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}