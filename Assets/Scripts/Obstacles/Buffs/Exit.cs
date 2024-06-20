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

    public void UseTrigger(GameObject player)
    {
        _gameplayService.LevelManager.LoadNextRoom(player);
    }

    public void DisableTrigger()
    {
    }
}

//public class TriggerBase : MonoBehaviour, IMyPoolable
//{
//    protected BoxCollider _collider;
//    protected bool _isActivated;
//    private void Start()
//    {
//        //_collider = GetComponent<BoxCollider>();
//        //_collider.isTrigger = false;
//    }

//    public void Activate()
//    {
//        _isActivated = true;
//    }
//    public void OnCreate()
//    {
//    }

//    public void OnPull()
//    {
//    }

//    public void OnRelease()
//    {
//    }
//}

//public class Exit : TriggerBase, ICompleatedRoomTrigger
//{
//    [SerializeField] private TriggerType _type;

//    [Inject]
//    private TriggerPooler _pooler;

//    [Inject]
//    private IDefaultScenario _scenario;

//    private void Start()
//    {
//        _isActivated = false;
//    }

//    public bool GetActiveStatus()
//    {
//        return _isActivated;
//    }

//    public void ActivateTrigger()
//    {
//        Debug.LogWarning("Exit trigger!");
//        _pooler.Init();
//    }

//    public void UseTrigger()
//    {
//        _scenario.LoadMainMenu();
//    }

//    public void DisableTrigger()
//    {
//        _pooler.Push(_type, this);
//    }
//}
