using Enemy;
using Player;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public enum TypeOfScenario
    {
        Default,
        Boss
    }
    public interface IGameplayService
    {
        void Init(TypeOfScenario type, IScenarioContext context);
        public IPlayerFactory _playerFactory { get; }
        public IStatsProvider _statsProvider { get; }
    }
    public class GameplayService : IGameplayService
    {

        [Inject]
        public IPlayerFactory _playerFactory { get; }

        [Inject]
        public IStatsProvider _statsProvider { get; }

        public IScenario ScenarioType;


        public GameplayService()
        {
            
        }


        public void Init(TypeOfScenario type, IScenarioContext context)
        {
            ScenarioType = ScenarioFactory.ReturnScenario(type,this,context);
            ScenarioType.Init();
        }
    }

    public class ScenarioFactory
    {
        // how to change this method so it would work when scenario will have generics
        public static IScenario ReturnScenario(TypeOfScenario type,IGameplayService fullService,IScenarioContext context)
        {
            IScenario scenario=null;
            switch (type)
            {
                case TypeOfScenario.Default:
                    scenario = new DefaultScenario(fullService, context);
                    break;
                case TypeOfScenario.Boss:
                    scenario = new BossScenario(fullService, context);
                    break;
            }
            return scenario;
        }
    }
}
