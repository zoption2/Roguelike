using Gameplay;
using Player;
using CharactersStats;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;
using Enemy;

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, PlayerType type, int id = -1);
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

    public IPlayerController CreatePlayer(Transform point, PlayerType type, int id = -1)
    {
        CharacterView playerView;
        IPlayerController controller;
        PlayerModel playerModel;
        Stats stats;

        controller = GetNewController();

        stats = _statsProvider.GetPlayerStats(type, id);

        playerModel = new PlayerModel(stats, type, id);

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

    //public abstract class Factory<TController, TType, TModel>
    //{
    //    protected readonly IStatsProvider _statsProvider;
    //    public abstract TController GetCharacter(TType type);
    //    protected abstract TModel GetNewModel();

    //}

    //public class PlayersFactory : Factory<IPlayerController, PlayerType, PlayerModel>
    //{
    //    public override IPlayerController GetCharacter(PlayerType type)
    //    {
    //        throw new System.NotImplementedException();
    //        var stats = _statsProvider.GetPlayerStats(type);
    //        var model = new PlayerModel(stats, type, 0);
    //    }

    //    protected override PlayerModel GetNewModel()
    //    {

    //    }
    //}

    //public class EnemiesFactory : Factory<IEnemyController, EnemyType, EnemyModel>
    //{

    //}
}