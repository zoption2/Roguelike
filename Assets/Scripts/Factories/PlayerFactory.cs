using Enemy;
using Gameplay;
using Player;
using Pool;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, PlayerType type, PlayerModel model, ICharacterScenarioContext characters);
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
        _controller.IsActive = true;
        _playerPooler = playerPooler;
    }

    public IPlayerController CreatePlayer(Transform point, PlayerType type, PlayerModel model, ICharacterScenarioContext characters)
    {
        Transform _poolableTransform;
        Rigidbody2D _playerViewRigidbody;

        _playerPooler.Init();

        var _poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        _playerView = _poolable.gameObject.GetComponent<PlayerView>();

        var _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
        _poolableTransform = _poolableObj.transform;

        _playerViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        _playerView.Initialize((PlayerController)_controller);

        _controller.Init(_poolableTransform, _playerViewRigidbody, model);

        characters.Players.Add(_controller);
        //_gameplayService.Players.Add(_controller);

        return _controller;
    }
}


