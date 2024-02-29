using Enemy;
using Player;
using Prefab;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface ISceneContextMarker
    {

    }
    public class MySceneContext : MonoBehaviour, ISceneContextMarker
    {
        [System.Serializable]
        public class PlayerSpawnPointWithType
        {
            public Transform spawnPoint;
            public PlayerType playerType;
        }

        [field: SerializeField] public List<PlayerSpawnPointWithType> PlayersSpawnPoints1 { get; set; }
        [field: SerializeField] public PlayerSpawnPointWithType[] PlayersSpawnPoints { get; set; }
        [field: SerializeField] public Transform[] EnemiesSpawnPoints { get; set; }
        public List<IPlayerController> Players { get; }
        public List<IEnemyController> Enemies { get; }


    }
}


