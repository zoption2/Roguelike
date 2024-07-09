using CharactersStats;
using Enemy;
using Player;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IGameplayService
    {
        GameObject Player { get; set; }
        IPoolManager PoolManager { get; set; }
        void Init(TypeOfScenario type);
        void StartCurrentRoom();
        IScenario Scenario { get; set; }
        IPlayerFactory _playerFactory { get; set; }
        IEnemyFactory _enemyFactory { get; set; }
        IStatsProvider _statsProvider { get; set; }
        IScenarioContext CurrentContext { get; set; }
        ILevelContext LevelContext { get; set; }

        void CheckIfAllStopped();
        void ProcessTurnEnd();

        event OnEndTurn ON_END_TURN;
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

        public event OnEndTurn ON_END_TURN;

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

            Scenario.Init(CurrentContext);
        }

        public void StartCurrentRoom()
        {
            if (LevelContext.CurrentRoomContext != null && LevelContext.CurrentRoomScenario != null)
            {
                Scenario = LevelContext.CurrentRoomScenario;
                CurrentContext = LevelContext.CurrentRoomContext;
                Scenario.Init(CurrentContext);
            }
            else
            {
                var currentRoomName = LevelContext.CurrentRoomName;
                var currentRoomType = LevelContext.CurrentRoomType;
                Scenario = _scenarioFactory.CreateScenario(currentRoomType, this);
                CurrentContext = new RoomContext();

                LevelContext.CreateRoomContext(currentRoomName);
                LevelContext.CreateScenario(currentRoomName, Scenario);
                LevelContext.CurrentRoomContext = LevelContext.GetRoomContext(currentRoomName);
                LevelContext.CurrentRoomScenario = LevelContext.GetScenario(currentRoomName);

                Scenario.Init(CurrentContext);
            }
        }

        public void CheckIfAllStopped()
        {
            foreach (IPlayerController player in LevelContext.CurrentRoomContext.Players)
            {
                if (player.CheckIfMoving())
                {
                    return;
                }
            }

            foreach (IEnemyController enemy in LevelContext.CurrentRoomContext.Enemies)
            {
                if (enemy.CheckIfMoving())
                {
                    return;
                }
            }
            ON_END_TURN?.Invoke();
        }

        public void ProcessTurnEnd()
        {
            foreach (IPlayerController player in LevelContext.CurrentRoomContext.Players)
            {
                player.UpdateHealthBar();
            }

            foreach (IEnemyController enemy in LevelContext.CurrentRoomContext.Enemies)
            {
                enemy.UpdateHealthBar();
            }
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
