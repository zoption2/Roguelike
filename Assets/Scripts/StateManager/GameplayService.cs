using CharactersStats;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IGameplayService
    {
        public IPoolManager PoolManager { get; set; }
        void Init(TypeOfScenario type);
        public IPlayerFactory _playerFactory { get; set; }
        public IEnemyFactory _enemyFactory { get; set; }
        public IStatsProvider _statsProvider { get; set; }
        public LevelManager LevelManager { get; set; }
    }

    public class GameplayService : IGameplayService
    {
        public IPoolManager PoolManager { get; set; }
        public IPlayerFactory _playerFactory { get; set; }
        public IEnemyFactory _enemyFactory { get; set; }
        public IStatsProvider _statsProvider { get; set; }
        public IScenarioFactory _scenarioFactory { get; set; }
        public IScenario ScenarioType;
        public LevelManager LevelManager { get; set; }

        [Inject]
        public void Construct(
            IPoolManager poolManager,
            IStatsProvider statsProvider,
            IScenarioFactory scenarioFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory)
        {
            PoolManager = poolManager;
            _scenarioFactory = scenarioFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            _statsProvider = statsProvider;    
        }

        public void Init(TypeOfScenario type)
        {
            PoolManager.InitPoolers();
            ScenarioType = _scenarioFactory.CreateScenario(type, this);
            IScenarioContext context = _scenarioFactory.CreateContext(type);
            ScenarioType.Init(context, LevelManager);
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
