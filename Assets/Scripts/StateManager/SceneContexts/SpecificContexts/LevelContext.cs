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
        //RoomContext CurrentRoomContext { get; set; }
        //IScenario CurrentRoomScenario { get; set; }
        //public string CurrentRoomName { get; set; }
        //public TypeOfScenario CurrentRoomType { get; set; }
        //public NavMeshSurface NavMeshSurface { get; set; }
        public void CleanAllContexts();
    }

    public class LevelContext : ILevelContext
    {
        public IPlayerController Player { get; set; }
        public RoomContext CurrentRoomContext { get; set; }
        public IScenario CurrentRoomScenario { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }

        public void CleanAllContexts()
        {
            Player = null;

            CurrentRoomContext?.ClearContext();
            CurrentRoomContext = null;
            CurrentRoomScenario = null;
            CurrentRoomName = null;
            CurrentRoomType = default;
        }

        public void ClearContext()
        {

        }
    }
}