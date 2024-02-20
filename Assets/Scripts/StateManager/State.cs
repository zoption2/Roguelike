using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Gameplay
{
    public interface IState
    {
        Scenario _scenario { get; }

        IGameplayService _gameplayService { get; }
        public void OnEnter();
        public void OnExit();
    }

    public class PlayerTurnState : IState
    {
        public Scenario _scenario { get; }

        public IGameplayService _gameplayService { get; }

        public PlayerTurnState(Scenario scenario, IGameplayService fullService)
        {
            _scenario = scenario;
            _gameplayService = fullService;
        }

        public void OnEnter()
        {
            for (int i = 0; i < _gameplayService.Players.Count; i++)
            {
                _gameplayService.Players[i].IsActive = true;
            }
            Debug.Log("Entered player turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _gameplayService.Players.Count; i++)
            {
                _gameplayService.Players[i].IsActive = false;
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
            }
            Debug.Log("Entered enemy turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _gameplayService.Enemies.Count; i++)
            {
                _gameplayService.Enemies[i].IsActive = false;
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
        }

        public void OnCreate()
        {

            int _id = 1;
            PlayerType _type = PlayerType.Warrior;
            Stats _stats = _gameplayService._statsProvider.GetStats(_type, _id);

            foreach (Transform spawnPoint in _gameplayService.PlayerSpawnPoints)
            {
                _gameplayService._playerFactory.CreatePlayer(spawnPoint, _type, new PlayerModel(_id ,_type, _stats));
            }

            Debug.Log(_stats.Health);
        }

        public void OnExit()
        {
            Debug.Log("Exited init state");
        }
    }

}
