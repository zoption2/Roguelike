using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;
using Prefab;
using Zenject;

namespace Gameplay
{
    public class BossScenario : Scenario
    {
        public override void Init(IPlayerFactory playerFactory = null)
        {
            //Init some stuff
        }

        public BossScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
        }
    }

    public class DefaultScenario : Scenario
    {
        public EnemyTurnState _enemyTurnState;
        public PlayerTurnState _playerTurnState;

        //[Inject]
        //private IPlayerFactory _playerFactory;

        public DefaultScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
            Debug.Log("This is the default scenario, so it starts with player turn");
        }
        public override void Init(IPlayerFactory playerFactory)
        {
            //initState
            _enemyTurnState = new EnemyTurnState(this, _gameplayService);
            _playerTurnState = new PlayerTurnState(this, _gameplayService);
            _currentState = _playerTurnState;//initState

            //Transform position = _gameplayService.PlayerSpawnPoints[0];

            foreach (Transform spawnPoint in _gameplayService.PlayerSpawnPoints)
            {
                playerFactory.CreatePlayer(spawnPoint, PlayerType.Warrior, new PlayerModel());
            }

            //_playerFactory.CreatePlayer(position, PlayerType.Warrior, new PlayerModel());

            PlayerController.OnSwitch += SwitchToEnemy;
            EnemyController.OnSwitch += SwitchToPlayer;
        }
        public void SwitchToEnemy()
        {
            SwitchState(_enemyTurnState);
        }
        public void SwitchToPlayer()
        {
            SwitchState(_playerTurnState);
        }
    }
    public abstract class Scenario
    {
        protected IState _currentState;

        public IGameplayService _gameplayService;

        public IState GetCurrentState()
        {
            return _currentState;
        }
        public Scenario()
        {
            
        }
        public abstract void Init(IPlayerFactory playerFactory = null);

        public void SwitchState(IState state)
        {
            if (_currentState != state)
            {
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter();
            }
        }
    }
}
