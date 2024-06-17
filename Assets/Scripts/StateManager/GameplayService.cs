using CharactersStats;
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
        void Init(TypeOfScenario type);
        public Queue<TypeOfScenario> RoomsOrder { get; set; }
        public void EnqueueScenario(TypeOfScenario scenario);
        public void InitRoom();
        public IPlayerFactory _playerFactory { get; }
        public IEnemyFactory _enemyFactory { get; }
        public IStatsProvider _statsProvider { get; }
    }

    public class GameplayService : IGameplayService
    {
        public Queue<TypeOfScenario> RoomsOrder { get; set; }

        public IPlayerFactory _playerFactory { get; }
        public IEnemyFactory _enemyFactory { get; }
        public IStatsProvider _statsProvider { get; }
        public IScenarioFactory _scenarioFactory { get; }
        public IScenario ScenarioType;

        public GameplayService(IStatsProvider statsProvider,
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
            ScenarioType.Init(context);
        }

        public void InitRoom()
        {
            if (RoomsOrder.Count > 0)
            {
                TypeOfScenario firstRoom = RoomsOrder.Dequeue();
                Init(firstRoom);
            }
            else
            {
                Debug.LogError("No rooms in the sequence to start the level.");
            }
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
                case TypeOfScenario.Default:
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
                case TypeOfScenario.Default:
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
