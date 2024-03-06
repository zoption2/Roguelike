//using CharactersStats;
//using Player;
//using Pool;
//using Prefab;
//using Zenject;

//public interface IPlayerFactory : ICharacterFactory<IPlayerController, PlayerType> { }
//public class PlayerFactory : CharacterFactory<IPlayerController, PlayerType>, IPlayerFactory
//{
//    private PlayerPooler _concretePooler;
//    protected override ObjectPooler<PlayerType> _pooler { get => _concretePooler; }

//    public PlayerFactory(
//        DiContainer container,
//        IStatsProvider statsProvider,
//        PlayerPooler playerPooler)
//        : base(container, statsProvider)
//    {
//        _concretePooler = playerPooler;
//        _concretePooler.Init();
//    }

//    protected override Stats GetStats(PlayerType playerType, int id)
//    {
//        return _statsProvider.GetCharacterStats(playerType, id);
//    }
//}

using Gameplay;
using Player;
using CharactersStats;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, PlayerType type, ICharacterScenarioContext characters, int id = 0);
}
public class PlayerFactory : IPlayerFactory
{
    private IStatsProvider _statsProvider;
    private DiContainer _container;
    private PlayerPooler _playerPooler;

    [Inject]
    public void Construct(
        DiContainer container,
        IStatsProvider statsProvider,
        PlayerPooler playerPooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _playerPooler = playerPooler;
        _playerPooler.Init();
    }

    public IPlayerController CreatePlayer(Transform point, PlayerType type, ICharacterScenarioContext characters, int id = 0)
    {
        CharacterView playerView;
        IPlayerController controller;
        PlayerModel playerModel;
        Stats stats;

        controller = GetNewController();

        stats = _statsProvider.GetPlayerStats(type, id);

        playerModel = new PlayerModel(id, type, stats);

        IMyPoolable _poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        playerView = _poolable.gameObject.GetComponent<CharacterView>();

        controller.Init(playerModel, playerView);

        Debug.Log($"Player {type} with id {id} was created. They have {stats.Health} hp and spawned on {point.position}");

        return controller;
    }

    private IPlayerController GetNewController()
    {
        return _container.Resolve<IPlayerController>();
    }
}