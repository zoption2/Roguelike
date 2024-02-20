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
        public override void Init()
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
        private EnemyTurnState _enemyTurnState;
        private PlayerTurnState _playerTurnState;
        private InitLevelState _initLevelState;

        public DefaultScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
            Debug.Log("This is the default scenario, so it starts with player turn");
        }
        public override void Init()
        {
            _enemyTurnState = new EnemyTurnState(this, _gameplayService);
            _playerTurnState = new PlayerTurnState(this, _gameplayService);
            _initLevelState = new InitLevelState(this, _gameplayService);

            _currentState = _initLevelState;
            _initLevelState.OnEnter();
            SwitchToPlayer();

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
        public abstract void Init();

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
