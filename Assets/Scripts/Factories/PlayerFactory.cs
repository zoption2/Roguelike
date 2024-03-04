using CharactersStats;
using Player;
using Pool;
using Prefab;
using Zenject;

public interface IPlayerFactory : ICharacterFactory<IPlayerController, PlayerType> { }
public class PlayerFactory : CharacterFactory<IPlayerController, PlayerType>, IPlayerFactory
{
    private PlayerPooler _concretePooler;
    protected override ObjectPooler<PlayerType> _pooler { get => _concretePooler; }

    public PlayerFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        PlayerPooler playerPooler)
        : base(container, statsProvider)
    {
        _concretePooler = playerPooler;
        _concretePooler.Init();
    }

    protected override Stats GetStats(PlayerType playerType, int id)
    {
        return _statsProvider.GetCharacterStats(playerType, id);
    }
}
