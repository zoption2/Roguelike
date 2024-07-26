using CharactersStats;
using Enemy;
using Player;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IGameplayService
    {
        public GameObject Player { get; set; }
        public IPoolManager PoolManager { get; set; }
        public IScenario CurrentScenario { get; set; }
        public IPlayerFactory PlayerFactory { get; set; }
        public IEnemyFactory EnemyFactory { get; set; }
        public IStatsProvider StatsProvider { get; set; }
        public RoomContext CurrentContext { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }
        public void Init(TypeOfScenario type);
        public void StartCurrentRoom();
        public void CheckIfAllStopped();
        public void ProcessTurnEnd();

        public event OnEndTurn ON_END_TURN;
    }

    public class GameplayService : IGameplayService
    {
        public GameObject Player { get; set; }
        public IPoolManager PoolManager { get; set; }
        public IPlayerFactory PlayerFactory { get; set; }
        public IEnemyFactory EnemyFactory { get; set; }
        public IStatsProvider StatsProvider { get; set; }
        public IScenarioFactory ScenarioFactory { get; set; }
        public IScenario CurrentScenario { get; set; }
        public RoomContext CurrentContext { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }

        private ILevelContext _levelContext;
        private ILevelManager _levelManager;

        public event OnEndTurn ON_END_TURN;

        [Inject]
        public void Construct(
            IPoolManager poolManager,
            IStatsProvider statsProvider,
            IScenarioFactory scenarioFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            ILevelContext levelContext,
            IRewardService rewardService,
            ILevelManager levelManager)
        {
            PoolManager = poolManager;
            ScenarioFactory = scenarioFactory;
            PlayerFactory = playerFactory;
            EnemyFactory = enemyFactory;
            StatsProvider = statsProvider;
            _levelContext = levelContext;
            _levelManager = levelManager;
            rewardService.Init();
        }

        public void Init(TypeOfScenario type)
        {
            CurrentScenario = ScenarioFactory.CreateScenario(type, this);
            CurrentContext = new RoomContext();

            CurrentScenario.Init(CurrentContext);
        }

        public void StartCurrentRoom()
        {
            string currentRoomName = CurrentRoomName;
            TypeOfScenario currentRoomType = CurrentRoomType;

            CurrentScenario = ScenarioFactory.CreateScenario(currentRoomType, this);

            if (_levelContext.Player == null)
            {
                CreatePlayer();
            }

            CurrentScenario.Init(CurrentContext);
        }

        public void CheckIfAllStopped()
        {
            foreach (IPlayerController player in CurrentContext.Players)
            {
                if (player.CheckIfMoving())
                {
                    return;
                }
            }

            foreach (IEnemyController enemy in CurrentContext.Enemies)
            {
                if (enemy.CheckIfMoving())
                {
                    return;
                }
            }
            Debug.LogWarning($"invoked ON_END_TURN for {CurrentScenario}  {CurrentScenario}");
            ON_END_TURN?.Invoke();
        }

        public void ProcessTurnEnd()
        {
            foreach (IPlayerController player in CurrentContext.Players)
            {
                player.UpdateHealthBar();
            }

            foreach (IEnemyController enemy in CurrentContext.Enemies)
            {
                enemy.UpdateHealthBar();
            }
        }

        public void CreatePlayer()
        {
            if (_levelContext.Player == null)
            {
                CharacterType playerType = DataTransfer.TypeCollection.FirstOrDefault();
                Vector3 newPos = new Vector3(0, 0, 0);
                Debug.LogWarning($"Creating new player at position: {newPos}");

                IPlayerController newPlayer = PlayerFactory.CreatePlayer(newPos, _levelManager.PlayerParent, playerType);
                newPlayer.SetCharacterContext(CurrentContext);
                CurrentContext.Players.Add(newPlayer);

                _levelContext.Player = newPlayer;
            }
            else
            {
                Debug.Log("Player already created!!!");
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
                    scenario = _diContainer.Resolve<IChestScenario>();
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
