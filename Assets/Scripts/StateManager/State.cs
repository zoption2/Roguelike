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

        public void OnEnter();
        public void OnExit();
    }

    public class PlayerTurnState : IState
    {
        public IScenario _scenario { get; }

        public ICharacterScenarioContext _characters { get; }

        public PlayerTurnState( IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            for (int i = 0; i < _characters.Players.Count; i++)
            {
                _characters.Players[i].IsActive = true;
                _characters.Players[i].OnEndTurn += _scenario.OnStateEnd;
            }
            Debug.Log("Entered player turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _characters.Players.Count; i++)
            {
                _characters.Players[i].IsActive = false;
                _characters.Players[i].OnEndTurn -= _scenario.OnStateEnd;
            }
            //Debug.Log("Exited player turn state");
        }
    }
    public class EnemyTurnState : IState
    {
        public IScenario _scenario { get; }
        
        public ICharacterScenarioContext _characters { get; }

        public EnemyTurnState(IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }
        public void OnEnter()
        {
            for(int i=0; i <_characters.Enemies.Count;i++ )
            {
                _characters.Enemies[i].IsActive = true;
                _characters.Enemies[i].OnEndTurn += _scenario.OnStateEnd;
            }
            Debug.Log("Entered enemy turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _characters.Enemies.Count; i++)
            {
                _characters.Enemies[i].IsActive = false;
                _characters.Enemies[i].OnEndTurn -= _scenario.OnStateEnd;
            }
            //Debug.Log("Exited enemy turn state");
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
            //Debug.Log("Entering init state");
            OnPlayerCreate();
            OnEnemyCreate();
            _scenario.OnStateEnd();
        }

        public void OnPlayerCreate()
        {
            //int id = 1;
            //foreach (PlayerSpawnPointWithType player in _characters.PlayerSpawnPoints)
            //{
            //    IPlayerController newPlayer = _playerFactory.CreateCharacter(player.spawnPoint, player.playerType);
            //    _characters.Players.Add(newPlayer);
            //}
            PlayerSpawnPointWithType player;
            PlayerType playerType;
            for (int i=0;i< _characters.PlayerSpawnPoints.Count; i++)
            {
                player = _characters.PlayerSpawnPoints[i];
                playerType = DataTransfer.TypeCollection[i];
                IPlayerController newPlayer = _playerFactory.CreatePlayer(player.spawnPoint, playerType);
                _characters.Players.Add(newPlayer);
            }

        }

        public void OnEnemyCreate()
        {
            foreach (EnemySpawnPointWithType enemy in _characters.EnemySpawnPoints)
            {
                IEnemyController newEnemy = _enemyFactory.CreateEnemy(enemy.spawnPoint, enemy.enemyType);
                _characters.Enemies.Add(newEnemy);
            }
        }

        public void OnExit()
        {
            //Debug.Log("Exited init state");
        }
    }

}
