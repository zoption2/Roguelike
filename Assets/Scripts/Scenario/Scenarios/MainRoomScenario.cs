using Enemy;
using Gameplay;
using Player;
using Pool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IMainRoomScenario : IScenario
{ 

}


public class MainRoomScenario : Scenario<RoomContext>, IMainRoomScenario
{
    private List<CookedMapper> _turnsOrder;
    private CharacterPooler _characterPooler;
    private CharacterUIPooler _characterUIPooler;
    private ProjectilePooler _projectilePooler;
    private EffectPooler _effectPooler;
    public MainRoomScenario(IGameplayService gameplayService, IStateFactory stateFactory, CharacterPooler characterPooler,
        CharacterUIPooler characterUIPooler, ProjectilePooler projectilePooler, EffectPooler effectPooler)
    {
        GameplayService = gameplayService;
        _queueOfStates = new Queue<IState>();
        _turnsOrder = new List<CookedMapper>();
        _stateFactory = stateFactory;
        _characterPooler = characterPooler;
        _characterUIPooler = characterUIPooler;
        _projectilePooler = projectilePooler;
        _effectPooler = effectPooler;
    }
    public override void EraseCharacter(ICharacterController controller)
    {
        Debug.LogError(controller + "was deleted");
        controller.ON_CHARACTER_DEATH -= EraseCharacter;
        controller.Dispose();
        foreach (CookedMapper mapper in _turnsOrder)
        {
            if (mapper.Controller == controller)
            {
                _turnsOrder.Remove(mapper);
                break;
            }
        }

        if (controller is IPlayerController)
        {
            _scenarioContext.Players.Remove((IPlayerController)controller);
        }
        else
        {
            _scenarioContext.Enemies.Remove((IEnemyController)controller);
        }
        CheckConditonsForEndOfScenario();
    }

    private void CleanPoolers()
    {
        _characterPooler.CleanPool();
        _characterUIPooler.CleanPool();
        _effectPooler.CleanPool();
        _projectilePooler.CleanPool();
        Debug.LogWarning("cleaned poolers!");
    }

    public override void CheckConditonsForEndOfScenario()
    {
        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        if (noPlayers)
        {
            foreach (ICharacterController enemy in _scenarioContext.Enemies)
            {
                enemy.ON_CHARACTER_DEATH -= EraseCharacter;
                enemy.Dispose();
            }
            LoadMainMenu();
        }
        else if (noEnemies)
        {
            Debug.LogWarning("You Won!");
            ActivateCompleatedRoomTriggers();
            //LoadMainMenu();
        }
    }

    public override void LoadMainMenu()
    {
        GameplayService.PoolManager.CleanPoolers();
        SceneManager.LoadScene("Menu");
    }

    public void ActivateCompleatedRoomTriggers()
    {
        Debug.LogWarning("Room Cleaned!");
        foreach (var trigger in _scenarioContext.CompleatedRoomTriggers)
        {
            trigger.ActivateTrigger();
        }
    }

    private void SubscribeToDeathOfCharacters()
    {
        foreach (ICharacterController controller in _scenarioContext.Players)
        {
            controller.ON_CHARACTER_DEATH += EraseCharacter;
        }

        foreach (ICharacterController controller in _scenarioContext.Enemies)
        {
            controller.ON_CHARACTER_DEATH += EraseCharacter;
        }
    }

    public override void Init(IScenarioContext context, LevelManager levelManager)
    {
        _levelManager = levelManager;
        SetScenarioContext(context);
        _stateFactory.Init(this, _scenarioContext);
        IState state = _stateFactory.CreateState(TypeOfState.Init);
        _queueOfStates.Enqueue(state);
        _currentState = _queueOfStates.Dequeue();
        _currentState.OnEnter();
    }

    private void GetSortedTurns()
    {
        DataTransfer.RawMappers = DataTransfer.RawMappers.OrderByDescending(x => x.Speed).ToList();
        foreach (RawMapper mapper in DataTransfer.RawMappers)
        {
            CookedMapper cookedMapper;
            cookedMapper = ConvertToCookedMapper(mapper.Controller);
            _turnsOrder.Add(cookedMapper);
        }
    }

    private CookedMapper ConvertToCookedMapper(ICharacterController characterController)
    {
        CookedMapper cookedMapper = new CookedMapper();
        cookedMapper.Controller = characterController;
        if (cookedMapper.Controller is IEnemyController)
        {
            cookedMapper.State = TypeOfState.EnemyTurn;
        }
        else
        {
            cookedMapper.State = TypeOfState.PlayerTurn;
        }
        return cookedMapper;
    }
    public override void RenewQueue()
    {
        if (_turnsOrder.Count == 0)
        {
            SubscribeToDeathOfCharacters();
            GetSortedTurns();
        }
        foreach (CookedMapper mapper in _turnsOrder)
        {
            IState state = _stateFactory.CreateState(mapper.State);
            state.SetCharacter(mapper.Controller);
            _queueOfStates.Enqueue(state);
        }
        //_scenarioContext.TeleportWallEnters.ForEach(teleportWallEnter => teleportWallEnter.Recharge());///
    }
}
