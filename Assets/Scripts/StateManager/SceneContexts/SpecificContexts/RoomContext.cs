using Enemy;
using Obstacles;
using Player;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public class PlayerSpawnPointWithType
    {
        public Vector3 SpawnPoint;
        public CharacterType Type;
    }

    public class EnemySpawnPointWithType
    {
        public Vector3 SpawnPoint;
        public CharacterType Type;
    }

    public class BuffSpawnPointWithType
    {
        public Vector3 SpawnPoint;
        public BuffType Type;
    }

    public interface IRoomContext : IScenarioContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<IBuff> Buffs { get; set; }
        public List<ICompleatedRoomTrigger> CompleatedRoomTriggers { get; set; }
        public List<IChestController> Chests { get; set; }
        public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }
        public List<BuffSpawnPointWithType> BuffSpawnPoints { get; set; }
        public List<TeleportWallEnter> TeleportWallEnters { get; set; }
        public Transform EnemiesParent { get; set; }
        public Transform BuffsParent { get; set; }
        
    }

    public class RoomContext : IRoomContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<ICompleatedRoomTrigger> CompleatedRoomTriggers { get; set; }
        public List<IChestController> Chests { get; set; }
        public List<IBuff> Buffs { get; set; }
        public List<TeleportWallEnter> TeleportWallEnters { get; set; }
        public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }
        public List<BuffSpawnPointWithType> BuffSpawnPoints { get; set; }
        public Transform EnemiesParent { get; set; }
        public Transform BuffsParent { get; set; }

        public RoomContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            Buffs = new List<IBuff>();
            CompleatedRoomTriggers = new List<ICompleatedRoomTrigger>();
            PlayerSpawnPoints = new List<PlayerSpawnPointWithType>();
            EnemySpawnPoints = new List<EnemySpawnPointWithType>();
            BuffSpawnPoints = new List<BuffSpawnPointWithType>();
            Chests = new List<IChestController>();
        }

        public void ClearContext()
        {
            Players.Clear();
            Enemies.Clear();
            Buffs.Clear();
            CompleatedRoomTriggers.Clear();
            PlayerSpawnPoints.Clear();
            EnemySpawnPoints.Clear();
            BuffSpawnPoints.Clear();
            Chests.Clear();
        }
    }
}