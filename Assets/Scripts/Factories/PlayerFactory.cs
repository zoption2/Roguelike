using Enemy;
using Gameplay;
using Player;
using CharactersStats;
using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Zenject;

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, PlayerType type, int id = 0);
}
public class PlayerFactory : IPlayerFactory
{
    private IPlayerController _controller;
    private IGameplayService _gameplayService;
    private IPlayerView _playerView;
    private ObjectPooler<PlayerType> _playerPooler;

    [Inject]
    public void Construct(
        IPlayerController controller,
        IGameplayService service,
        ObjectPooler<PlayerType> playerPooler
        )
    {
        _controller = controller;
        _gameplayService = service;
        _playerPooler = playerPooler;
    }

    public IPlayerController CreatePlayer(Transform point, PlayerType type, int id = 0)
    {
        Transform _poolableTransform;
        Rigidbody2D _playerViewRigidbody;
        PlayerModel _playerModel;
        Stats _stats;

        _playerPooler.Init();

        _stats = _gameplayService._statsProvider.GetPlayerStats(type, id);

        _playerModel = new PlayerModel(id, type, _stats);

        var _poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        _playerView = _poolable.gameObject.GetComponent<PlayerView>();

        var _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
        _poolableTransform = _poolableObj.transform;

        _playerViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        _playerView.Initialize((PlayerController)_controller);

        _controller.Init(_poolableTransform, _playerViewRigidbody, _playerModel);

        _gameplayService.Players.Add(_controller);

        Debug.Log($"Player with id {id} was created. They have {_stats.Health} hp and spawned on {point.position}");

        return _controller;
    }
}


