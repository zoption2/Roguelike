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
        public static Dictionary<TypeOfState, IState> GetStateSet<T>(BaseScenario<T> scenarioInstance) where T : ISceneContextMarker
        {
            Dictionary<TypeOfState, IState> stateSet = new Dictionary<TypeOfState, IState>();
            Debug.Log(scenarioInstance.GetType());
            switch (scenarioInstance)
            {
                case TestScenario:
                    stateSet.Add(TypeOfState.Init, new InitLevelState(scenarioInstance));
                    stateSet.Add(TypeOfState.PlayerTurn, new PlayerTurnState(scenarioInstance));
                    stateSet.Add(TypeOfState.EnemyTurn, new EnemyTurnState(scenarioInstance));
                    break;               
            }
            return stateSet;
        }
    }
}
