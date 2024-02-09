using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;

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
        public EnemyTurnState _enemyTurnState;
        public PlayerTurnState _playerTurnState;

        public DefaultScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
            Debug.Log("This is the default scenario, so it starts wit player turn");
        }
        public override void Init()
        {
            _enemyTurnState = new EnemyTurnState(this, _gameplayService);
            _playerTurnState = new PlayerTurnState(this, _gameplayService);
            _currentState = _playerTurnState;
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
