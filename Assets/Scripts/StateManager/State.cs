using CharactersStats;
using Enemy;
using Player;
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

            _characterController.UpdateHealthBar();

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

            _characterController.UpdateHealthBar();
            
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

        IPlayerFactory _playerFactory;

        IEnemyFactory _enemyFactory;

        INavigationFactory _navigationFactory;


        public InitLevelState(IScenario scenario,
            ICharacterScenarioContext context,
            IStatsProvider provider,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            INavigationFactory navigationFactory)
        {
            _scenario = scenario;
            _characters = context;
            _statsProvider  = provider;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            _navigationFactory = navigationFactory;
        }

        public void OnEnter()
        {
            OnPlayerCreate();
            OnEnemyCreate();
            OnNavigationCreate();
            _scenario.OnStateEnd();
        }
        public void OnNavigationCreate()
        {
            NavMeshSurface navMeshSurface = _navigationFactory.CreateNavigation();
            _characters.NavMeshSurface  = navMeshSurface;
            navMeshSurface.BuildNavMesh();
        }

        public void OnPlayerCreate()
        {
            PlayerSpawnPointWithType player;
            CharacterType playerType;
            for (int i=0;i< _characters.PlayerSpawnPoints.Count; i++)
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
            foreach (EnemySpawnPointWithType enemy in _characters.EnemySpawnPoints)
            {
                IEnemyController newEnemy = _enemyFactory.CreateEnemy(enemy.spawnPoint, enemy.enemyType);
                newEnemy.SetCharacterContext(_characters);
                _characters.Enemies.Add(newEnemy);
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
