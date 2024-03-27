using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public enum TypeOfState
    {
        Init,
        PlayerTurn,
        EnemyTurn
    }

    public interface IStateFactory
    {
        public void Init(IScenario scenarioInstance, ICharacterScenarioContext context);
        public IState CreateState(TypeOfState type);
    }
    public class StateData : IStateFactory
    {
        private IPlayerFactory _playerFactory;
        private IEnemyFactory _enemyFactory;
        private IStatsProvider _statsProvider;
        private IScenario _scenarioInstance;
        private ICharacterScenarioContext _context;

        [Inject]
        public void Construct(IPlayerFactory playerFactory, IEnemyFactory enemyFactory, IStatsProvider statsProvider)
        {
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
        }

        public void Init(IScenario scenarioInstance, ICharacterScenarioContext context)
        {
            _context = context;
            _scenarioInstance = scenarioInstance;
        }


        public IState CreateState(TypeOfState type)
        {
            IState state = null;
            switch (type)
            {
                case TypeOfState.Init:
                    state =  new InitLevelState(_scenarioInstance, _context, _statsProvider, _playerFactory, _enemyFactory);
                    break;
                case TypeOfState.PlayerTurn:
                    state = new PlayerTurnState(_scenarioInstance, _context);
                    break;
                case TypeOfState.EnemyTurn:
                    state = new EnemyTurnState(_scenarioInstance, _context);
                    break;
            }
            return state;
        }
    }
}
