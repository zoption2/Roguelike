using CharactersStats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
    

    public interface IGameplayService
    {
        void Init(TypeOfScenario type);
        public Queue<TypeOfScenario> RoomsOrder { get; set; }
        public void EnqueueScenario(TypeOfScenario scenario);
        public IPlayerFactory _playerFactory { get; }
        public IEnemyFactory _enemyFactory { get; }
        public IStatsProvider _statsProvider { get; }
        public LevelManager LevelManager { get; set; }
    }

    public class GameplayService : IGameplayService
    {
        public Queue<TypeOfScenario> RoomsOrder { get; set; }

        public IPlayerFactory _playerFactory { get; }
        public IEnemyFactory _enemyFactory { get; }
        public IStatsProvider _statsProvider { get; }
        public IScenarioFactory _scenarioFactory { get; }
        public IScenario ScenarioType;
        public LevelManager LevelManager { get; set; }

        public GameplayService(
            IStatsProvider statsProvider,
            IScenarioFactory scenarioFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            List<TypeOfScenario> scenarios)
        {
            _scenarioFactory = scenarioFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            _statsProvider = statsProvider;

            RoomsOrder = new Queue<TypeOfScenario>(scenarios);
        }

        public void Init(TypeOfScenario type)
        {
            ScenarioType = _scenarioFactory.CreateScenario(type, this);
            IScenarioContext context = _scenarioFactory.CreateContext(type);
            ScenarioType.Init(context, LevelManager);
        }

        public void EnqueueScenario(TypeOfScenario context)
        {
            RoomsOrder.Enqueue(context);
        }
    }

    public interface IScenarioFactory
    {
        public IScenario CreateScenario(TypeOfScenario type, IGameplayService fullService);
        public IScenarioContext CreateContext(TypeOfScenario type);
    }

    public class ScenarioFactory : IScenarioFactory
    {
        [Inject]
        public DiContainer _diContainer;

        public IScenario CreateScenario(TypeOfScenario type, IGameplayService fullService)
        {
            IScenario scenario = null;
            switch (type)
            {
                case TypeOfScenario.DefaultRoom:
                    scenario = _diContainer.Resolve<IDefaultScenario>();
                    break;
                case TypeOfScenario.MainRoom:
                    scenario = _diContainer.Resolve<IDefaultScenario>();
                    break;
                    //case TypeOfScenario.Boss:
                    //    //scenario = new BossScenario(fullService, context);
                    //    scenario = new BossScenario(fullService);
                    //    break;
            }
            return scenario;
        }

        public IScenarioContext CreateContext(TypeOfScenario type)
        {
            IScenarioContext context = null;

            switch (type)
            {
                case TypeOfScenario.DefaultRoom:
                    context = new DefaultScenarioContext();
                    break;
                case TypeOfScenario.MainRoom:
                    context = new DefaultScenarioContext();
                    break;
                default:
                    Debug.LogWarning("--|" + this + "Can`t create a scenario context |--");
                    break;
            }

            return context;
        }

        
    }
}
