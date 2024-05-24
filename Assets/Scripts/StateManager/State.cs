using CharactersStats;
using Enemy;
using Obstacles;
using Player;
using System;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public interface IState
    {
        public IScenario _scenario { get; }
        public void SetCharacter(ICharacterController controller);
        public void OnEnter();
        public void OnExit();
    }


    public class PlayerTurnState : IState
    {
        public IScenario _scenario { get; }
        public ICharacterScenarioContext _characters { get; }

        private ICharacterController _characterController;

        public PlayerTurnState( IScenario scenario, ICharacterScenarioContext context)
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
        public IScenario _scenario { get; }
        public ICharacterScenarioContext _characters { get; }

        private ICharacterController _characterController;

        public EnemyTurnState(IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            Debug.Log($"-----------------------------|Enemy {_characterController.GetCharacterType()}|-------------------------------");
            _characterController.IsActive = true;

            _characters.ON_END_TURN += _scenario.OnStateEnd;

            _characterController.UseEffectsOnStart();
            _characterController.AnalizeCondition();

            _characterController.ProcessOnStartTurn();

            _characterController.Tick();

            if (!_characters.Enemies.Contains(_characterController))
                _scenario.OnStateEnd();
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
        public IScenario _scenario { get; }

        public ICharacterScenarioContext _characters { get; }

        IStatsProvider _statsProvider;

        IBuffFactory _buffFactory;

        IPlayerFactory _playerFactory;

        IEnemyFactory _enemyFactory;

        INavigationFactory _navigationFactory;


        public InitLevelState(IScenario scenario,
            ICharacterScenarioContext context,
            IStatsProvider provider,
            IBuffFactory buffFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            INavigationFactory navigationFactory)
        {
            _scenario = scenario;
            _characters = context;
            _statsProvider  = provider;
            _buffFactory = buffFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            _navigationFactory = navigationFactory;
        }

        public void OnEnter()
        {
            OnPlayerCreate();
            OnEnemyCreate();
            OnBuffCreate();
            OnNavigationCreate();
            _scenario.OnStateEnd();
        }

        public void OnNavigationCreate()
        {
            NavMeshSurface navMeshSurface = _navigationFactory.CreateNavigation();
            _characters.NavMeshSurface  = navMeshSurface;
            navMeshSurface.BuildNavMesh();
        }

        public void OnBuffCreate()
        {
            var buffTypes = Enum.GetValues(typeof(BuffType)).Cast<BuffType>().Where(t => t != BuffType.None).ToList();
            var shuffledSpawnPoints = _characters.BuffSpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

            foreach (var spawnPointWithType in shuffledSpawnPoints)
            {
                BuffType buffType = spawnPointWithType.buffType;

                if (buffType != BuffType.None)
                {
                    IBuff newBuff = _buffFactory.CreateBuff(spawnPointWithType.spawnPoint, buffType);
                    _characters.Buffs.Add(newBuff);
                }
            }

            foreach (var spawnPointWithType in shuffledSpawnPoints)
            {
                BuffType buffType = spawnPointWithType.buffType;

                if (buffType == BuffType.None)
                {
                    buffType = buffTypes[UnityEngine.Random.Range(0, buffTypes.Count)];

                    if (_characters.Buffs.Any(b => b.GetBuffType() == buffType))
                    {
                        continue;
                    }

                    IBuff newBuff = _buffFactory.CreateBuff(spawnPointWithType.spawnPoint, buffType);
                    float probability = newBuff.GetBuffProbability();

                    if (UnityEngine.Random.value <= probability)
                    {
                        _characters.Buffs.Add(newBuff);
                    }
                    else
                    {
                        newBuff.RemoveBuff();
                    }
                }
            }
        }

        public void OnPlayerCreate()
        {
            PlayerSpawnPointWithType player;
            CharacterType playerType;
            for (int i = 0; i < _characters.PlayerSpawnPoints.Count; i++)
            {
                player = _characters.PlayerSpawnPoints[i];
                playerType = DataTransfer.TypeCollection[i];
                IPlayerController newPlayer = _playerFactory.CreatePlayer(player.spawnPoint, playerType);
                newPlayer.SetCharacterContext(_characters);
                _characters.Players.Add(newPlayer);
            }
        }

        public void OnEnemyCreate()
        {
            var enemyTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Where(t => t != EnemyType.None).ToList();
            int enemyCount = UnityEngine.Random.Range(1, _characters.EnemySpawnPoints.Count + 1);
            var shuffledSpawnPoints = _characters.EnemySpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

            foreach (var spawnPointWithType in shuffledSpawnPoints)
            {
                CharacterType enemyType = spawnPointWithType.enemyType;

                if (enemyType != CharacterType.None)
                {
                    IEnemyController newEnemy = _enemyFactory.CreateEnemy(spawnPointWithType.spawnPoint, enemyType);
                    newEnemy.SetCharacterContext(_characters);
                    _characters.Enemies.Add(newEnemy);
                }
            }

            for (int i = 0; i < enemyCount; i++)
            {
                var spawnPointWithType = shuffledSpawnPoints[i];
                CharacterType enemyType = spawnPointWithType.enemyType;

                if (enemyType == CharacterType.None)
                {
                    enemyType = (CharacterType)enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];

                    IEnemyController newEnemy = _enemyFactory.CreateEnemy(spawnPointWithType.spawnPoint, enemyType);
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

}
