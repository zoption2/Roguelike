using Enemy;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class PlayerSpawnPointWithType
    {
        public Transform spawnPoint;
        public PlayerType playerType;
    }

    [System.Serializable]
    public class EnemySpawnPointWithType
    {
        public Transform spawnPoint;
        public EnemyType enemyType;
    }
    public interface ICharacterScenarioContext : IScenarioContext
    {
        public List<IPlayerController> Players { get;  set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }
    }
    [System.Serializable]
    public class DefaultScenarioContext : MonoBehaviour, ICharacterScenarioContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        [field: SerializeField] public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        [field: SerializeField] public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }

        public DefaultScenarioContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            PlayerSpawnPoints = new List<PlayerSpawnPointWithType>();
            EnemySpawnPoints = new List<EnemySpawnPointWithType>();
        }
    }
}
