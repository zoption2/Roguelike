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
        IPlayerController Player { get; set; }
        RoomContext GetRoomContext(string roomName);
        void CreateRoomContext(string roomName);
        void RemoveRoomContext(string roomName);
        IScenario GetScenario(string roomName);
        void CreateScenario(string roomName, IScenario scenario);
        void RemoveScenario(string roomName);
        RoomContext CurrentRoomContext { get; set; }
        IScenario CurrentRoomScenario { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }

        public void CleanAllContexts();
        public void CleanContexts();
    }

    public class LevelContext : ILevelContext
    {
        public IPlayerController Player { get; set; }
        private Dictionary<string, RoomContext> _contexts;
        private Dictionary<string, IScenario> _scenarios;
        public RoomContext CurrentRoomContext { get; set; }
        public IScenario CurrentRoomScenario { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }

        public LevelContext()
        {
            _contexts = new Dictionary<string, RoomContext>();
            _scenarios = new Dictionary<string, IScenario>();
        }

        public void CleanContexts()
        {
            _contexts.Clear();
        }

        public void CleanAllContexts()
        {
            foreach (var context in _contexts.Values)
            {
                context.Players.Clear();
                context.Enemies.Clear();
                context.Buffs.Clear();
                context.CompleatedRoomTriggers.Clear();
                context.Chests.Clear();
                context.PlayerSpawnPoints.Clear();
                context.EnemySpawnPoints.Clear();
                context.BuffSpawnPoints.Clear();
                context.TeleportWallEnters.Clear();
            }
            _contexts.Clear();

            CurrentRoomContext = null;
            CurrentRoomScenario = null;
            CurrentRoomName = null;
            CurrentRoomType = default;
        }

        public RoomContext GetRoomContext(string roomName)
        {
            if (_contexts.TryGetValue(roomName, out var context))
            {
                return context;
            }
            return null;
        }

        public void CreateRoomContext(string roomName)
        {
            if (!_contexts.ContainsKey(roomName))
            {
                _contexts.Add(roomName, new RoomContext());
                Debug.Log($"Room context created for room: {roomName}");
            }
        }

        public void RemoveRoomContext(string roomName)
        {
            if (_contexts.ContainsKey(roomName))
            {
                _contexts.Remove(roomName);
                Debug.Log($"Room context removed for room: {roomName}");
            }
        }

        public IScenario GetScenario(string roomName)
        {
            if (_scenarios.TryGetValue(roomName, out var scenario))
            {
                return scenario;
            }
            return null;
        }

        public void CreateScenario(string roomName, IScenario scenario)
        {
            if (!_scenarios.ContainsKey(roomName))
            {
                _scenarios.Add(roomName, scenario);
                Debug.Log($"Scenario created for room: {roomName}");
            }
        }

        public void RemoveScenario(string roomName)
        {
            if (_scenarios.ContainsKey(roomName))
            {
                _scenarios.Remove(roomName);
                Debug.Log($"Scenario removed for room: {roomName}");
            }
        }
    }
}