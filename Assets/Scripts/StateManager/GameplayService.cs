using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public enum TypeOfScenario
    {
        Default,
        Boss
    }
    public interface IGameplayService
    {
        public List<ICharacterController> Players { get; }
        public List<ICharacterController> Enemies { get; }
        void Init(TypeOfScenario type);
    }
    public class GameplayService : IGameplayService
    {
        public List<ICharacterController> Players { get; }
        public List<ICharacterController> Enemies { get; }
        public Scenario ScenarioType;

        public GameplayService()
        {
            Players = new List<ICharacterController>();
            Enemies = new List<ICharacterController>();
        }


        public void Init(TypeOfScenario type)
        {
            ScenarioType = ScenarioFactory.ReturnScenario(type,this);
            ScenarioType.Init();
            //ScenarioType._fullService = this;
        }
    }

    public class ScenarioFactory
    {
        public static Scenario ReturnScenario(TypeOfScenario type,IGameplayService fullService)
        {
            Scenario scenario=null;
            switch (type)
            {
                case TypeOfScenario.Default:
                    scenario = new DefaultScenario(fullService);
                    break;
                case TypeOfScenario.Boss:
                    scenario = new BossScenario(fullService);
                    break;
            }
            return scenario;
        }
    }
}
