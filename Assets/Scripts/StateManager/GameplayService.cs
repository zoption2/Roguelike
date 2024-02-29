using CharactersStats;
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

        public IPlayerFactory _playerFactory { get; }

        public IStatsProvider _statsProvider { get; }

        public IScenarioFactory _scenarioFactory { get; }

        public IScenario ScenarioType;

        public GameplayService(IStatsProvider statsProvider,IScenarioFactory scenarioFactory,IPlayerFactory playerFactory)
        {
            _scenarioFactory = scenarioFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
        }  

        public void Init(TypeOfScenario type, IScenarioContext context)
        {
            ScenarioType = _scenarioFactory.ReturnScenario(type,this,context);
            ScenarioType.Init(context);
        }
    }

    public interface IScenarioFactory
    {
        public IScenario ReturnScenario(TypeOfScenario type, IGameplayService fullService, IScenarioContext context);
    }

    public class ScenarioFactory : IScenarioFactory
    {
        [Inject]
        public DiContainer _diContainer;
        public  IScenario ReturnScenario(TypeOfScenario type,IGameplayService fullService,IScenarioContext context)
        {
            IScenario scenario=null;
            switch (type)
            {
                case TypeOfScenario.Default:
                    //_container.Resolve<IPlayerController>();
                    scenario = _diContainer.Resolve<IDefaultScenario>();
                    //scenario = new DefaultScenario(fullService, context);// DIContainer.Reslove<DefaultScenario>
                    break;
                case TypeOfScenario.Boss:
                    scenario = new BossScenario(fullService, context);
                    break;
            }
            return scenario;
        }
    }
}
