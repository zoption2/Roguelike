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
    IStatsProvider _statsProvider;
    DiContainer _container;
    ObjectPooler<PlayerType> _playerPooler;

    [Inject]
    public void Construct(
        DiContainer container,
        IStatsProvider statsProvider,
        ObjectPooler<PlayerType> playerPooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _playerPooler = playerPooler;
        _playerPooler.Init();
    }

    public IPlayerController CreatePlayer(Transform point, PlayerType type, ICharacterScenarioContext characters, int id = 0)
    {
        IPlayerView playerView;
        IPlayerController controller;
        Transform poolableTransform;
        Rigidbody2D playerViewRigidbody;
        PlayerModel playerModel;
        Stats stats;

        controller = GetNewController();

        stats = _statsProvider.GetPlayerStats(type, id);

        playerModel = new PlayerModel(id, type, stats);

        IMyPoolable _poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        playerView = _poolable.gameObject.GetComponent<PlayerView>();

        GameObject _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
        poolableTransform = _poolableObj.transform;

        playerViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        playerView.Initialize((PlayerController)controller);

        controller.Init(poolableTransform, playerViewRigidbody, playerModel);

        characters.Players.Add(controller);

        Debug.Log($"Player with id {id} was created. They have {stats.Health} hp and spawned on {point.position}");

        return controller;
    }

    private IPlayerController GetNewController()
    {
        return _container.Resolve<IPlayerController>();
    }
}