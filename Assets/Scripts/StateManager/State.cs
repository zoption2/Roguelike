using CharactersStats;
using Cysharp.Threading.Tasks;
using Enemy;
using Obstacles;
using Player;
using Pool;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public interface IState
    {
        public IScenario Scenario { get; }
        public void SetCharacter(ICharacterController controller);
        public ICharacterController GetCharacter();
        public void OnEnter();
        public void OnExit();
    }


    public class PlayerTurnState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }
        public RoomContext _roomContext { get; }

        private ICharacterController _characterController;

        public PlayerTurnState( IScenario scenario, RoomContext context)
        {
            _scenario = scenario;
            _roomContext = context;
        }

        public void OnEnter()
        {
            Debug.Log($"-----------------------------|Player {_characterController.GetCharacterType()}|-------------------------------");
            _characterController.IsActive = true;

            _scenario.GameplayService.ON_END_TURN -= _scenario.OnStateEnd;
            _scenario.GameplayService.ON_END_TURN += _scenario.OnStateEnd;
            //_roomContext.ON_END_TURN += _scenario.OnStateEnd;


            _characterController.UseEffectsOnStart();
            _characterController.AnalizeCondition();

            _characterController.ProcessOnStartTurn();

            if (_characterController.IsStunned)
            {
                _scenario.OnStateEnd();
            }
        }

        public void OnExit()
        {
            _characterController.IsActive = false;

            _characterController.UseEffectsOnEnd();
            _characterController.AnalizeCondition();

            _characterController.ProcessOnEndTurn();
            
            _scenario.GameplayService.ProcessTurnEnd();
            //_roomContext.ProcessTurnEnd();

            _scenario.GameplayService.ON_END_TURN -= _scenario.OnStateEnd;
            //_roomContext.ON_END_TURN -= _scenario.OnStateEnd;
            Debug.Log("----------------------------|EXIT|--------------------------------");
        }

        public void SetCharacter(ICharacterController controller)
        {
            _characterController = controller;
        }

        public ICharacterController GetCharacter()
        {
            return _characterController;
        }
    }
    public class EnemyTurnState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }
        public RoomContext _roomContext { get; }

        private ICharacterController _characterController;

        public EnemyTurnState(IScenario scenario, RoomContext context)
        {
            _scenario = scenario;
            _roomContext = context;
        }

        public void OnEnter()
        {
            Debug.Log($"-----------------------------|Enemy {_characterController.GetCharacterType()}|-------------------------------");
            
            _characterController.IsActive = true;

            _scenario.GameplayService.ON_END_TURN += _scenario.OnStateEnd;
            //_roomContext.ON_END_TURN += _scenario.OnStateEnd;

            _characterController.UseEffectsOnStart();
            _characterController.AnalizeCondition();

            _characterController.ProcessOnStartTurn();

            _characterController.Tick();

            
        }

        public void OnExit()
        {
            _characterController.IsActive = false;

            _characterController.UseEffectsOnEnd();
            _characterController.AnalizeCondition();
            
            _characterController.ProcessOnEndTurn();

            _scenario.GameplayService.ProcessTurnEnd();
            //_roomContext.ProcessTurnEnd();

            _scenario.GameplayService.ON_END_TURN -= _scenario.OnStateEnd;
            //_roomContext.ON_END_TURN -= _scenario.OnStateEnd;
            Debug.Log("----------------------------|EXIT|--------------------------------");
        }

        public void SetCharacter(ICharacterController controller)
        {
            _characterController = controller;
        }

        public ICharacterController GetCharacter()
        {
            return _characterController;
        }
    }

    public class InitLevelState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }

        public RoomContext _roomContext { get; }

        private const float YOffset = 0.5f;

        IStatsProvider _statsProvider;

        IBuffFactory _buffFactory;

        IPlayerFactory _playerFactory;

        IEnemyFactory _enemyFactory;

        INavigationFactory _navigationFactory;

        IRoomBuilder _roomBuilder;
        ILevelManager _levelManager;
        ILevelContext _levelContext;


        public InitLevelState(
            IScenario scenario,
            RoomContext context,
            IStatsProvider provider,
            IBuffFactory buffFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            INavigationFactory navigationFactory,
            IRoomObjectsFactory roomObjectsFactory,
            ILevelManager levelManager,
            ILevelContext levelContext,
            IRoomBuilder roomBuilder)
        {
            _scenario = scenario;
            _roomContext = context;
            _statsProvider  = provider;
            _buffFactory = buffFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;  
            _levelManager = levelManager;
            _levelContext = levelContext;
            _roomBuilder = roomBuilder;
            //_navigationFactory = navigationFactory;
            
        }

        public void OnEnter()
        {
            RoomTemplateSO.Template template = _levelManager.GetTemplate();

            //if (template == null)
            //{
            //    Debug.LogError("Template not found");
            //    return;
            //}

            //_roomBuilder.BuildLevel(template);
            //OnBuffCreate();
            OnPlayerCreate();
            OnEnemyCreate();

        }

        public  void OnBuffCreate()
        {
            var buffTypes = Enum.GetValues(typeof(BuffType)).Cast<BuffType>().Where(t => t != BuffType.None).ToList();
            var shuffledSpawnPoints = _roomContext.BuffSpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

            foreach (var spawnPointWithType in shuffledSpawnPoints)
            {
                BuffType buffType = spawnPointWithType.Type;

                if (buffType != BuffType.None)
                {
                    Vector3 newPos = spawnPointWithType.SpawnPoint;
                    IBuff newBuff = _buffFactory.CreateBuff(newPos, _roomContext.BuffsParent, buffType);
                    _roomContext.Buffs.Add(newBuff);
                }
            }
        }

        public void OnPlayerCreate()
        {
            if (_levelContext.Player != null)
            {
                PlayerSpawnPointWithType playerSpawnPoint = _roomContext.PlayerSpawnPoints.FirstOrDefault();
                if (playerSpawnPoint != null)
                {
                    Vector3 newPos = new Vector3(playerSpawnPoint.SpawnPoint.x, playerSpawnPoint.SpawnPoint.y + YOffset, playerSpawnPoint.SpawnPoint.z);
                    Debug.LogWarning($"Moving player to position: {newPos}");
                    _levelContext.Player.GetTransform().position = newPos;
                }
                else
                {
                    Debug.LogError("No player spawn points available in the current room context.");
                }
            }
            else
            {
                Debug.LogError("Player has not been created yet!");
            }
        }




        public void OnEnemyCreate()
        {
            for (int i = 0; i < _roomContext.EnemySpawnPoints.Count; i++)
            {
                var spawnPointWithType = _roomContext.EnemySpawnPoints[i];
                CharacterType enemyType = spawnPointWithType.Type;

                if (enemyType != CharacterType.None)
                {
                    Vector3 newPos = new Vector3(spawnPointWithType.SpawnPoint.x, spawnPointWithType.SpawnPoint.y + YOffset, spawnPointWithType.SpawnPoint.z);
                    IEnemyController newEnemy = _enemyFactory.CreateEnemy(newPos, _roomContext.EnemiesParent, enemyType);
                    newEnemy.SetCharacterContext(_roomContext);
                    _roomContext.Enemies.Add(newEnemy);
                }
            }
        }

        public void OnExit()
        {
        }

        public void SetCharacter(ICharacterController controller)
        {

        }

        public ICharacterController GetCharacter()
        {
            return null;
        }
    }

    public class PauseState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }

        public PauseState(IScenario scenario)
        {
            _scenario = scenario;
        }

        public void OnEnter()
        {
            Debug.Log($"-----------------------------|ENTER PAUSE STATE FOR {_scenario}|-------------------------------");
        }

        public void OnExit()
        {
            Debug.Log($"-----------------------------|EXIT PAUSE STATE FOR {_scenario}|-------------------------------");
        }

        public void SetCharacter(ICharacterController controller)
        {
        }

        public ICharacterController GetCharacter()
        {
            return null;
        }
    }

}
