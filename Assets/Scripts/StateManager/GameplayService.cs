using CharactersStats;
using Cinemachine;
using DG.Tweening;
using Enemy;
using Player;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
        public ILevelContext LevelContext { get; set; }
        public void StartCurrentRoom();
        public void CheckIfAllStopped();
        public void ProcessTurnEnd();
        public void SetLevelManager(ILevelManager levelManager);

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

        public ILevelContext LevelContext { get; set; }
        private ILevelManager _levelManager;
        private CinemachineVirtualCamera _virtualCamera;

        public event OnEndTurn ON_END_TURN;

        public GameplayService(
            IPoolManager poolManager,
            IStatsProvider statsProvider,
            IScenarioFactory scenarioFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            ILevelContext levelContext,
            IRewardService rewardService)
        {
            PoolManager = poolManager;
            ScenarioFactory = scenarioFactory;
            PlayerFactory = playerFactory;
            EnemyFactory = enemyFactory;
            StatsProvider = statsProvider;
            LevelContext = levelContext;

            rewardService.Init();
        }

        public void SetLevelManager(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        




        public void StartCurrentRoom()
        {
            CurrentScenario = ScenarioFactory.CreateScenario(CurrentRoomType, this);

            //if (LevelContext.Player == null)
            //{
            //    CreatePlayer();
            //}

            CurrentScenario.Init(CurrentContext);
            Debug.LogWarning("Next room was inited");
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

        //public void CreatePlayer()
        //{
        //    if (LevelContext.Player == null)
        //    {
        //        CharacterType playerType = DataTransfer.TypeCollection.FirstOrDefault();
        //        Vector3 newPos = new Vector3(0, 0, 0);
        //        Debug.LogWarning($"Creating new player at position: {newPos}");

        //        IPlayerController newPlayer = PlayerFactory.CreatePlayer(newPos, _levelManager.PlayerParent, playerType);
        //        newPlayer.SetCharacterContext(CurrentContext);
        //        CurrentContext.Players.Add(newPlayer);

        //        LevelContext.Player = newPlayer;
        //    }
        //    else
        //    {
        //        Debug.Log("Player already created!!!");
        //    }
        //}

    }
}
