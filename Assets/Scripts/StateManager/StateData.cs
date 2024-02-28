using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public enum TypeOfState
    {
        Init,
        PlayerTurn,
        EnemyTurn
    }
    public class StateData
    {
        // how to change this method so it would work when scenario will have generics
        public static Dictionary<TypeOfState, IState> GetStateSet(IScenario scenarioInstance, ICharacterScenarioContext gameplayService)
        {
            Dictionary<TypeOfState, IState> stateSet = new Dictionary<TypeOfState, IState>();
            Debug.Log(scenarioInstance.GetType());
            switch (scenarioInstance)
            {
                case DefaultScenario:
                    stateSet.Add(TypeOfState.Init, new InitLevelState(scenarioInstance, gameplayService));
                    stateSet.Add(TypeOfState.PlayerTurn, new PlayerTurnState(scenarioInstance, gameplayService));
                    stateSet.Add(TypeOfState.EnemyTurn, new EnemyTurnState(scenarioInstance, gameplayService));
                    break;
            }
            return stateSet;
        }
    }
}
