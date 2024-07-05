using Gameplay;
using Obstacles;
using Pool;
using UnityEngine;
using Zenject;

public class Exit : MonoBehaviour, ICompleatedRoomTrigger
{
    private bool _isActivated;
    private BoxCollider _boxCollider;
    private IGameplayService _gameplayService;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Init(IGameplayService gameplayService)
    {
        _gameplayService = gameplayService;
    }

    public bool GetActiveStatus()
    {
        return _isActivated;
    }

    public void ActivateTrigger()
    {
        _boxCollider.isTrigger = true;
    }

    public void UseTrigger()
    {
        _gameplayService.CurrentContext = null;
        if(_gameplayService.LevelManager.RoomsOrder.Count > 0)
        {
            _gameplayService.Scenario.Pause();
            _gameplayService.LevelManager.LoadNextRoom();
        } else
        {
            _gameplayService.Scenario.LoadMainMenu();
        }
        
    }

    public void DisableTrigger()
    {
    }
}
