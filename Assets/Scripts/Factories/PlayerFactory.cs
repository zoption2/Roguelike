using Enemy;
using Gameplay;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, PlayerType type, PlayerModel model);
}
public class PlayerFactory : IPlayerFactory
{
    private IPlayerController _controller;
    private IGameplayService _gameplayService;

    [Inject]
    public void Construct(IPlayerController controller, IGameplayService service)
    {
        _controller = controller;
        _gameplayService = service;
        _controller.IsActive = true;
    }

    public IPlayerController CreatePlayer(Transform point, PlayerType type, PlayerModel model)
    {
        _controller.Init(point, type, model);
        _gameplayService.Players.Add(_controller);
        return _controller;
    }
}


