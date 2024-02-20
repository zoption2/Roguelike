using Enemy;
using Player;
using System.Collections;
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
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }
        public List<Transform> PlayerSpawnPoints { get; }
        public List<Transform> EnemySpawnPoints { get; }
        void Init(TypeOfScenario type);
    }
    public class GameplayService : IGameplayService
    {
        [Inject]
        private IPlayerFactory _playerFactory;
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }
        public List<Transform> PlayerSpawnPoints { get; }
        public List<Transform> EnemySpawnPoints { get; }
        public Scenario ScenarioType;

        public GameplayService()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            PlayerSpawnPoints = new List<Transform>();
            EnemySpawnPoints = new List<Transform>();
        }


        public void Init(TypeOfScenario type)
        {
            Debug.Log("Hello!&!&!&");
            ScenarioType = ScenarioFactory.ReturnScenario(type,this);
            ScenarioType.Init(_playerFactory);
            //ScenarioType._fullService = this;
        }
    }

    public class ScenarioFactory
    {
        public static Scenario ReturnScenario(TypeOfScenario type,IGameplayService fullService)
        {
            Scenario scenario=null;
            switch (type)
            {
                case TypeOfScenario.Default:
                    scenario = new DefaultScenario(fullService);
                    break;
                case TypeOfScenario.Boss:
                    scenario = new BossScenario(fullService);
                    break;
            }
            return scenario;
        }
    }
}
