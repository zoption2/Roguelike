using Gameplay;
using Obstacles;
using UnityEngine;

public class Exit : MonoBehaviour, ICompleatedRoomTrigger
{
    private bool _isActivated;
    private BoxCollider _boxCollider;
    private IGameplayService _gameplayService;
    private ILevelManager _levelManager;
    private TemplateElementType _exitType;
    private ExitDirection _exitDirection; // Додане поле для напрямку виходу

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Init(IGameplayService gameplayService, ILevelManager levelManager)
    {
        _gameplayService = gameplayService;
        _levelManager = levelManager;
    }

    public void SetExitType(TemplateElementType exitType)
    {
        _exitType = exitType;
    }

    public TemplateElementType GetExitType()
    {
        return _exitType;
    }

    public void SetExitDirection(ExitDirection exitDirection)
    {
        _exitDirection = exitDirection;
    }

    public ExitDirection GetExitDirection()
    {
        return _exitDirection;
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
        _gameplayService.CurrentScenario.Pause();
        _levelManager.SwitchToNextRoom();
    }

    public void DisableTrigger()
    {
    }
}
