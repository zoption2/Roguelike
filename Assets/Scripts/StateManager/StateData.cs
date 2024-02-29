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

    public interface IStateData
    {
        Dictionary<TypeOfState, IState> GetStateSet(IScenario scenarioInstance, ICharacterScenarioContext context);
    }
    public class StateData : IStateData
    {
        private IPlayerFactory _playerFactory;
        private IEnemyFactory _enemyFactory;
        private IStatsProvider _statsProvider;

        [Inject]
        public void Construct(IPlayerFactory playerFactory, IEnemyFactory enemyFactory, IStatsProvider statsProvider)
        {
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
        }
        // how to change this method so it would work when scenario will have generics
        public Dictionary<TypeOfState, IState> GetStateSet(IScenario scenarioInstance, ICharacterScenarioContext context)
        {

            Dictionary<TypeOfState, IState> stateSet = new Dictionary<TypeOfState, IState>();
            Debug.Log(scenarioInstance.GetType());
            switch (scenarioInstance)
            {
                case DefaultScenario:
                    stateSet.Add(TypeOfState.Init, new InitLevelState(scenarioInstance, context, _statsProvider, _playerFactory, _enemyFactory));
                    stateSet.Add(TypeOfState.PlayerTurn, new PlayerTurnState(scenarioInstance, context));
                    stateSet.Add(TypeOfState.EnemyTurn, new EnemyTurnState(scenarioInstance, context));
                    break;
            }
            return stateSet;
        }
    }
}
