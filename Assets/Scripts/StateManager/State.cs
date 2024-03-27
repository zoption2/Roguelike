using CharactersStats;
using Enemy;
using Player;
using Prefab;
using UnityEngine;

namespace Gameplay
{
    public interface IState
    {
        IScenario _scenario { get; }

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
            _characterController.IsActive = true;
            _characterController.OnEndTurn += _scenario.OnStateEnd;
            Debug.Log("Entered player turn state");
        }

        public void OnExit()
        {
            _characterController.IsActive = false;
            _characterController.OnEndTurn -= _scenario.OnStateEnd;
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
            _characterController.IsActive = true;
            _characterController.OnEndTurn += _scenario.OnStateEnd;
            Debug.Log("Entered enemy turn state");
            _characterController.Tick();
        }

        public void OnExit()
        {
            _characterController.IsActive = false;
            _characterController.OnEndTurn -= _scenario.OnStateEnd;
            Debug.Log("Exited enemy turn state");
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


        public InitLevelState(IScenario scenario,
            ICharacterScenarioContext context,
            IStatsProvider provider,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory)
        {
            _scenario = scenario;
            _characters = context;
            _statsProvider  = provider;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
        }

        public void OnEnter()
        {
            OnPlayerCreate();
            OnEnemyCreate();
            _scenario.OnStateEnd();
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
            //Debug.Log("Exited init state");
        }

        public void SetCharacter(ICharacterController controller)
        {

        }
    }

}
