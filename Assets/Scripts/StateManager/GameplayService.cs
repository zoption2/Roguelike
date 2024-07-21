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
        GameObject Player { get; set; }
        IPoolManager PoolManager { get; set; }
        void Init(TypeOfScenario type);
        void StartCurrentRoom();
        IScenario CurrentScenario { get; set; }
        IPlayerFactory _playerFactory { get; set; }
        IEnemyFactory _enemyFactory { get; set; }
        IStatsProvider _statsProvider { get; set; }
        IScenarioContext CurrentContext { get; set; }
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
        public IScenario CurrentScenario { get; set; }
        public IScenarioContext CurrentContext { get; set; }
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
            ILevelManager levelManager)
        {
            PoolManager = poolManager;
            _scenarioFactory = scenarioFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
            _statsProvider = statsProvider;
            _levelContext = levelContext;
            _levelManager = levelManager;
        }

        public void Init(TypeOfScenario type)
        {
            CurrentScenario = _scenarioFactory.CreateScenario(type, this);
            CurrentContext = new RoomContext();

            CurrentScenario.Init(CurrentContext);
        }

        public void StartCurrentRoom()
        {
            string currentRoomName = _levelContext.CurrentRoomName;
            TypeOfScenario currentRoomType = _levelContext.CurrentRoomType;

            IScenario currentScenario = _scenarioFactory.CreateScenario(currentRoomType, this);
            _levelContext.CreateScenario(currentRoomName, currentScenario);

            _levelContext.CurrentRoomContext = _levelContext.GetRoomContext(currentRoomName);
            _levelContext.CurrentRoomScenario = _levelContext.GetScenario(currentRoomName);

            CurrentScenario = _levelContext.GetScenario(currentRoomName);
            CurrentContext = _levelContext.CurrentRoomContext;

            if (_levelContext.Player == null)
            {
                CreatePlayer();
            }

            
            CurrentScenario.Init(CurrentContext);
            _levelManager.BuildNextRooms();
        }



        public void CheckIfAllStopped()
        {
            foreach (IPlayerController player in _levelContext.CurrentRoomContext.Players)
            {
                if (player.CheckIfMoving())
                {
                    return;
                }
            }

            foreach (IEnemyController enemy in _levelContext.CurrentRoomContext.Enemies)
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
            foreach (IPlayerController player in _levelContext.CurrentRoomContext.Players)
            {
                player.UpdateHealthBar();
            }

            foreach (IEnemyController enemy in _levelContext.CurrentRoomContext.Enemies)
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

                GameObject playerParentObject = new GameObject("Player");

                IPlayerController newPlayer = _playerFactory.CreatePlayer(newPos, playerParentObject.transform, playerType);
                newPlayer.SetCharacterContext(_levelContext.CurrentRoomContext);
                _levelContext.CurrentRoomContext.Players.Add(newPlayer);

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
