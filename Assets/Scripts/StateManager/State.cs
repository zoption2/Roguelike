using Enemy;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay
{

    public class DefaultScenarioContext : ISceneContextMarker
    {
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }
        public List<Transform> PlayerSpawnPoints { get; }
        public List<Transform> EnemySpawnPoints { get; }

        public void CompleteState()
        {

        }


    }


    public interface IState
    {
        Scenario _scenario { get; }

        IGameplayService _gameplayService { get; }
        public void OnEnter();
        public void OnExit();
    }

    public interface IEnemyCreateState : IState
    {
        public void OnEnemyCreate();
    }

    public interface IPlayerCreateState : IState
    {
        public void OnPlayerCreate();
    }

    public class PlayerTurnState : IState
    {
        public Scenario _scenario { get; }

        public IGameplayService _gameplayService { get; }

        public PlayerTurnState( Scenario scenario, IGameplayService fullService)
        {
            _scenario = scenario;
            _gameplayService = fullService;
        }

        public void OnEnter()
        {
            for (int i = 0; i < _gameplayService.Players.Count; i++)
            {
                _gameplayService.Players[i].IsActive = true;
                _gameplayService.Players[i].OnSwitch += _scenario.OnStateEnd;
            }
            Debug.Log("Entered player turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _gameplayService.Players.Count; i++)
            {
                _gameplayService.Players[i].IsActive = false;
                _gameplayService.Players[i].OnSwitch -= _scenario.OnStateEnd;
            }
            Debug.Log("Exited player turn state");
        }
    }
    public class EnemyTurnState : IState
    {
        public Scenario _scenario { get; }
        
        public IGameplayService _gameplayService { get; }

        public EnemyTurnState(Scenario scenario, IGameplayService fullService)
        {
            _scenario = scenario;
            _gameplayService = fullService;
        }
        public void OnEnter()
        {
            for(int i=0; i <_gameplayService.Enemies.Count;i++ )
            {
                _gameplayService.Enemies[i].IsActive = true;
                _gameplayService.Enemies[i].OnSwitch += _scenario.OnStateEnd;
            }
            Debug.Log("Entered enemy turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _gameplayService.Enemies.Count; i++)
            {
                _gameplayService.Enemies[i].IsActive = false;
                _gameplayService.Enemies[i].OnSwitch -= _scenario.OnStateEnd;
            }
            Debug.Log("Exited enemy turn state");
        }
    }

    public class InitLevelState : IState
    {
        public Scenario _scenario { get; }

        public IGameplayService _gameplayService { get; }

        public InitLevelState(Scenario scenario, IGameplayService fullService)
        {
            _scenario = scenario;
            _gameplayService = fullService;
        }

        public void OnEnter()
        {
            Debug.Log("Entering init state");
            OnCreate();
            _scenario.OnStateEnd();
        }

        public void OnCreate()
        {
            // TODO: create a correct logic for player (and enemies) generation
            foreach (Transform spawnPoint in _gameplayService.PlayerSpawnPoints)
            {
                _gameplayService._playerFactory.CreatePlayer(spawnPoint, PlayerType.Warrior, 1);
            }

            foreach (Transform spawnPoint in _gameplayService.EnemySpawnPoints)
            {
                _gameplayService._enemyFactory.CreateEnemy(spawnPoint, EnemyType.Barbarian);
            }
        }

        public void OnExit()
        {
            Debug.Log("Exited init state");
        }
    }

}
