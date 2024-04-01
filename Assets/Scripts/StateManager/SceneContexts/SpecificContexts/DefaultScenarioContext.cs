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
        public CharacterType playerType;
    }

    [System.Serializable]
    public class EnemySpawnPointWithType
    {
        public Transform spawnPoint;
        public CharacterType enemyType;
    }
    public interface ICharacterScenarioContext : IScenarioContext
    {
        public List<IPlayerController> Players { get;  set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }
        public void CheckIfAllStopped();

        public event OnEndTurn ON_END_TURN;
    }
    [System.Serializable]
    public class DefaultScenarioContext : MonoBehaviour, ICharacterScenarioContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        [field: SerializeField] public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        [field: SerializeField] public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }

        public event OnEndTurn ON_END_TURN;

        public DefaultScenarioContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            PlayerSpawnPoints = new List<PlayerSpawnPointWithType>();
            EnemySpawnPoints = new List<EnemySpawnPointWithType>();
        }

        public void CheckIfAllStopped()
        {
            foreach (IPlayerController player in Players)
            {
                if (player.CheckIfMoving())
                {
                    return;
                }
            }
            foreach(IEnemyController enemy in Enemies)
            {
                if (enemy.CheckIfMoving())
                {
                    return;
                }
                    
            }
            ON_END_TURN?.Invoke();
        }
    }
}
