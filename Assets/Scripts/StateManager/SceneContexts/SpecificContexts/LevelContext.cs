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
        RoomContext CurrentRoom { get; set; }
    }

    public class LevelContext : ILevelContext
    {
        public GameObject Player { get; set; }
        private Dictionary<string, RoomContext> _contexts;
        public RoomContext CurrentRoom { get; set; }

        public LevelContext()
        {
            _contexts = new Dictionary<string, RoomContext>();
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
    }
}