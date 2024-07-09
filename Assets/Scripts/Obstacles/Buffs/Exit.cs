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
    private ILevelManager _levelManager;    

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    [Inject]
    public void Construct(
        IGameplayService gameplayService,
        ILevelManager levelManager)
    {
        _gameplayService = gameplayService;
        _levelManager = levelManager;
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
        if(_levelManager.RoomsOrder.Count > 0)
        {
            _gameplayService.CurrentScenario.Pause();
            //_gameplayService.LevelManager.LoadNextRoom();
        } else
        {
            _gameplayService.CurrentScenario.LoadMainMenu();
        }
        
    }

    public void DisableTrigger()
    {
    }
}
