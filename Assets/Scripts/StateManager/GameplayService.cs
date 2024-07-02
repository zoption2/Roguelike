using CharactersStats;
using Player;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IGameplayService
    {
        public GameObject Player { get; set; }
        public IPoolManager PoolManager { get; set; }
        void Init(TypeOfScenario type);
        public IScenario Scenario { get; set; }
        public IPlayerFactory _playerFactory { get; set; }
        public IEnemyFactory _enemyFactory { get; set; }
        public IStatsProvider _statsProvider { get; set; }
        public IScenarioContext CurrentContext { get; set; }
        public ILevelContext LevelContext { get; set; }
        public LevelManager LevelManager { get; set; }
    }

    public class GameplayService : IGameplayService
    {
        public GameObject Player { get; set; }
        public IPoolManager PoolManager { get; set; }
        public IPlayerFactory _playerFactory { get; set; }
        public IEnemyFactory _enemyFactory { get; set; }
        public IStatsProvider _statsProvider { get; set; }
        public IScenarioFactory _scenarioFactory { get; set; }
        public IScenario Scenario { get; set; }
        public IScenarioContext CurrentContext { get; set; }
        public ILevelContext LevelContext { get; set; }
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
            Scenario = _scenarioFactory.CreateScenario(type, this);
            CurrentContext = new RoomContext();
            
            Scenario.Init(CurrentContext, LevelManager);
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
                    context = new RoomContext();
                    break;
                case TypeOfScenario.MainRoom:
                    context = new RoomContext();
                    break;
                default:
                    Debug.LogWarning("--|" + this + "Can`t create a scenario context |--");
                    break;
            }

            return context;
        }

        
    }
}
