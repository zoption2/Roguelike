using Enemy;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface ICharacterScenarioContext : IScenarioContext
    {
        public List<IPlayerController> Players { get;  set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<Transform> PlayerSpawnPoints { get; set; }
        public List<Transform> EnemySpawnPoints { get; set; }
    }
    [System.Serializable]
    public class DefaultScenarioContext : MonoBehaviour, ICharacterScenarioContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        [field: SerializeField] public List<Transform> PlayerSpawnPoints { get; set; }
        [field: SerializeField] public List<Transform> EnemySpawnPoints { get; set; }

        public DefaultScenarioContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            PlayerSpawnPoints = new List<Transform>();
            EnemySpawnPoints = new List<Transform>();
        }
    }
}
