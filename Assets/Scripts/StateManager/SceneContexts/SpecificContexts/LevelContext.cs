using Enemy;
using Obstacles;
using Player;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public interface ILevelContext : IScenarioContext
    {
        GameObject Player { get; set; }
        RoomContext GetRoomContext(string roomName);
        void CreateRoomContext(string roomName);
        void RemoveRoomContext(string roomName);
    }

    public class LevelContext : ILevelContext
    {
        public GameObject Player { get; set; }
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
        public List<ICompleatedRoomTrigger> CompleatedRoomTriggers { get; set; }
        public List<IBuff> Buffs { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }
        public List<TeleportWallEnter> TeleportWallEnters { get; set; }
        public List<PlayerSpawnPointWithType> PlayerSpawnPoints { get; set; }
        public List<EnemySpawnPointWithType> EnemySpawnPoints { get; set; }
        public List<BuffSpawnPointWithType> BuffSpawnPoints { get; set; }

        public event OnEndTurn ON_END_TURN;

        private Dictionary<string, RoomContext> _roomContexts;

        public LevelContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
            Buffs = new List<IBuff>();
            CompleatedRoomTriggers = new List<ICompleatedRoomTrigger>();
            PlayerSpawnPoints = new List<PlayerSpawnPointWithType>();
            EnemySpawnPoints = new List<EnemySpawnPointWithType>();
            BuffSpawnPoints = new List<BuffSpawnPointWithType>();
            _roomContexts = new Dictionary<string, RoomContext>();
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

            foreach (IEnemyController enemy in Enemies)
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
            foreach (IPlayerController player in Players)
            {
                player.UpdateHealthBar();
            }

            foreach (IEnemyController enemy in Enemies)
            {
                enemy.UpdateHealthBar();
            }
        }

        public void ChangeRoom()
        {
        }

        public void CreateRoomContext(string roomName)
        {
            if (!_roomContexts.ContainsKey(roomName))
            {
                _roomContexts.Add(roomName, new RoomContext());
            }
        }

        public RoomContext GetRoomContext(string roomName)
        {
            if (_roomContexts.TryGetValue(roomName, out var context))
            {
                return context;
            }
            return null;
        }

        public void RemoveRoomContext(string roomName)
        {
            if (_roomContexts.ContainsKey(roomName))
            {
                _roomContexts.Remove(roomName);
            }
        }
    }
}
