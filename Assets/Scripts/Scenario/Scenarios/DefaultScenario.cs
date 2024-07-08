using Enemy;
using Gameplay;
using Player;
using Pool;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;
using System.Linq;

public interface IDefaultScenario : IScenario
{
    //public void LoadMainMenu();
}

public class DefaultScenario : Scenario<DefaultScenarioContext>, IDefaultScenario
{
    private List<CookedMapper> _turnsOrder;
    private CharacterPooler _characterPooler;
    private CharacterUIPooler _characterUIPooler;
    private ProjectilePooler _projectilePooler;
    private EffectPooler _effectPooler;
    public DefaultScenario(IGameplayService gameplayService, IStateFactory stateFactory, CharacterPooler characterPooler,
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

    protected void RemoveCharacterFromTurnsOrder(ICharacterController controller)
    {
        foreach (CookedMapper mapper in _turnsOrder)
        {
            if (mapper.Controller == controller)
            {
                _turnsOrder.Remove(mapper);
                break;
            }
        }
    }

    protected void RemoveCharacterFromDataTransfer(ICharacterController controller)
    {
        foreach (RawMapper mapper in DataTransfer.RawMappers)
        {
            if (mapper.Controller == controller)
            {
                DataTransfer.RawMappers.Remove(mapper);
                break;
            }
        }
    }


    public override void EraseCharacter(ICharacterController controller)
    {
        controller.ON_CHARACTER_DEATH -= EraseCharacter;
        controller.Dispose();
        RemoveCharacterFromTurnsOrder(controller);
        RemoveCharacterFromDataTransfer(controller);

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
            ActivateCompleatedRoomTriggers();
            //LoadMainMenu();
        }
    }

    public override void LoadMainMenu()
    {
        //CleanPoolers();
        //_levelManager.LoadNextRoom();
        //SceneManager.LoadScene("Menu");
    }

    public void ActivateCompleatedRoomTriggers()
    {
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

        SubscribeToDeathOfCharacters();
        SortTurns();

        OnStateEnd();
    }

    private void SortTurns()
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
        foreach (CookedMapper mapper in _turnsOrder)
        {
            IState state = _stateFactory.CreateState(mapper.State);
            state.SetCharacter(mapper.Controller);
            _queueOfStates.Enqueue(state);
        }
    }
}