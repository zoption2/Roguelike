using Gameplay;
using Obstacles;
using UnityEngine;

public class Exit : MonoBehaviour, ICompleatedRoomTrigger
{
    public Transform Transform { get; set; }

    [SerializeField]
    private TemplateElementType _exitType;

    [SerializeField]
    private ExitDirection _exitDirection;

    private bool _isActivated;
    private BoxCollider _boxCollider;
    private IGameplayService _gameplayService;
    private ILevelManager _levelManager;
    

    public void Init(IGameplayService gameplayService, ILevelManager levelManager)
    {
        _gameplayService = gameplayService;
        _levelManager = levelManager;
        _boxCollider = GetComponent<BoxCollider>();
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
        Debug.LogWarning("Exit from level");
        _gameplayService.CurrentScenario.ClearTurnOrder();
        _gameplayService.CurrentScenario.ClearTurnQueue();
        _gameplayService.CurrentScenario.HandleRoomChange();
        _gameplayService.LevelContext.Player.StopPlayer();
        _gameplayService.LevelContext.Player.IsMoving = false;
        _levelManager.BuildNextRoom();
    }

    public void DisableTrigger()
    {
    }
}
