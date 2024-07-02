using CharactersStats;
using Enemy;
using Obstacles;
using Player;
using Pool;
using System;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject.SpaceFighter;

namespace Gameplay
{
    public interface IState
    {
        public IScenario Scenario { get; }
        public void SetCharacter(ICharacterController controller);
        public void OnEnter();
        public void OnExit();
    }


    public class PlayerTurnState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }
        public IRoomContext _characters { get; }

        private ICharacterController _characterController;

        public PlayerTurnState( IScenario scenario, IRoomContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            Debug.Log($"-----------------------------|Player {_characterController.GetCharacterType()}|-------------------------------");
            _characterController.IsActive = true;

            _characters.ON_END_TURN += _scenario.OnStateEnd;


            _characterController.UseEffectsOnStart();
            _characterController.AnalizeCondition();

            _characterController.ProcessOnStartTurn();


            if (!_characters.Players.Contains(_characterController) || _characterController.IsStunned)
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

            _characters.ProcessTurnEnd();

            _characters.ON_END_TURN -= _scenario.OnStateEnd;
            Debug.Log("----------------------------|EXIT|--------------------------------");
        }

        public void SetCharacter(ICharacterController controller)
        {
            _characterController = controller;
        }
    }
    public class EnemyTurnState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }
        public IRoomContext _characters { get; }

        private ICharacterController _characterController;

        public EnemyTurnState(IScenario scenario, IRoomContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            if (!_characters.Enemies.Contains(_characterController))
                _scenario.OnStateEnd();
            Debug.Log($"-----------------------------|Enemy {_characterController.GetCharacterType()}|-------------------------------");
            _characterController.IsActive = true;

            _characters.ON_END_TURN += _scenario.OnStateEnd;

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

            _characters.ProcessTurnEnd();

            _characters.ON_END_TURN -= _scenario.OnStateEnd;
            Debug.Log("----------------------------|EXIT|--------------------------------");
        }

        public void SetCharacter(ICharacterController controller)
        {
            _characterController = controller;
        }
    }

    public class InitLevelState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }

        public IRoomContext _characters { get; }

        private const float YOffset = 0.5f;

        IStatsProvider _statsProvider;

        IBuffFactory _buffFactory;

        IPlayerFactory _playerFactory;

        IEnemyFactory _enemyFactory;

        INavigationFactory _navigationFactory;

        IRoomBuilder _roomBuilder;


        public InitLevelState(
            IScenario scenario,
            IRoomContext context,
            IStatsProvider provider,
            IBuffFactory buffFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            INavigationFactory navigationFactory,
            IRoomObjectsFactory roomObjectsFactory)
        {
            _scenario = scenario;
            _characters = context;
            _statsProvider  = provider;
            _buffFactory = buffFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;  
            //_navigationFactory = navigationFactory;
            _roomBuilder = new RoomBuilder(
                scenario,
                context, 
                navigationFactory,
                roomObjectsFactory
                );
        }

        public void OnEnter()
        {
            RoomTemplateSO.Template template = _scenario.GameplayService.LevelManager.GetTemplate();

            if (template == null)
            {
                Debug.LogError("Template not found");
                return;
            }

            _roomBuilder.BuildLevel(template);
            OnPlayerCreate();
            OnEnemyCreate();
            OnBuffCreate();
            _scenario.OnStateEnd();
        }

        public void OnBuffCreate()
        {
            var buffTypes = Enum.GetValues(typeof(BuffType)).Cast<BuffType>().Where(t => t != BuffType.None).ToList();
            var shuffledSpawnPoints = _characters.BuffSpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

            foreach (var spawnPointWithType in shuffledSpawnPoints)
            {
                BuffType buffType = spawnPointWithType.Type;

                if (buffType != BuffType.None)
                {
                    Vector3 newPos = spawnPointWithType.SpawnPoint;
                    IBuff newBuff = _buffFactory.CreateBuff(newPos, _roomBuilder.BuffsParent, buffType);
                    _characters.Buffs.Add(newBuff);
                }
            }
        }

        public void OnPlayerCreate()
        {
            if (_scenario.GameplayService.Player == null)
            {
                PlayerSpawnPointWithType player;
                CharacterType playerType;
                for (int i = 0; i < _characters.PlayerSpawnPoints.Count; i++)
                {
                    player = _characters.PlayerSpawnPoints[i];
                    playerType = DataTransfer.TypeCollection[i];
                    Vector3 newPos = new Vector3(player.SpawnPoint.x, player.SpawnPoint.y + YOffset, player.SpawnPoint.z);
                    Debug.LogWarning(newPos);
                    IPlayerController newPlayer = _playerFactory.CreatePlayer(newPos, _roomBuilder.PlayersParent, playerType);
                    newPlayer.SetCharacterContext(_characters);
                    _scenario.GameplayService.Player = _roomBuilder.PlayersParent.gameObject;
                    Debug.LogWarning(_scenario.GameplayService.Player);
                    _scenario.GameplayService.LevelContext.Players.Add(newPlayer);
                    _characters.Players.Add(newPlayer);
                }
            }
            else
            {
                Debug.LogError("Player already created. Moving to spawn point.");

                PlayerSpawnPointWithType playerSpawnPoint = _characters.PlayerSpawnPoints[0];
                Vector3 newPos = new Vector3(playerSpawnPoint.SpawnPoint.x, playerSpawnPoint.SpawnPoint.y + YOffset, playerSpawnPoint.SpawnPoint.z);

                _scenario.GameplayService.Player.transform.position = newPos;
                _scenario.GameplayService.Player.transform.SetParent(_roomBuilder.PlayersParent);

                IPlayerController existingPlayerController = _scenario.GameplayService.Player.GetComponent<IPlayerController>();
                if (existingPlayerController != null)
                {
                    existingPlayerController.SetCharacterContext(_characters);
                    if (!_characters.Players.Contains(existingPlayerController))
                    {
                        _characters.Players.Add(existingPlayerController);
                    }
                    if (!_scenario.GameplayService.LevelContext.Players.Contains(existingPlayerController))
                    {
                        _scenario.GameplayService.LevelContext.Players.Add(existingPlayerController);
                    }
                }
            }
        }


        public void OnEnemyCreate()
        {
            for (int i = 0; i < _characters.EnemySpawnPoints.Count; i++)
            {
                var spawnPointWithType = _characters.EnemySpawnPoints[i];
                CharacterType enemyType = spawnPointWithType.Type;

                if (enemyType != CharacterType.None)
                {
                    Vector3 newPos = new Vector3(spawnPointWithType.SpawnPoint.x, spawnPointWithType.SpawnPoint.y + YOffset, spawnPointWithType.SpawnPoint.z);
                    IEnemyController newEnemy = _enemyFactory.CreateEnemy(newPos, _roomBuilder.EnemiesParent, enemyType);
                    newEnemy.SetCharacterContext(_characters);
                    _characters.Enemies.Add(newEnemy);
                }
            }
        }

        public void OnExit()
        {
        }

        public void SetCharacter(ICharacterController controller)
        {

        }
    }

    public class PauseState : IState
    {
        private IScenario _scenario;

        public IScenario Scenario { get { return _scenario; } }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void SetCharacter(ICharacterController controller)
        {
        }
    }

}
