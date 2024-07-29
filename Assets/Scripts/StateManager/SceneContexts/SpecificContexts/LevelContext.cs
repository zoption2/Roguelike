using Player;
using Unity.AI.Navigation;

namespace Gameplay
{
    public interface ILevelContext : IScenarioContext
    {
        IPlayerController Player { get; set; }
        public void CleanAllContexts();
    }

    public class LevelContext : ILevelContext
    {
        public IPlayerController Player { get; set; }
        public RoomContext CurrentRoomContext { get; set; }
        public string CurrentRoomName { get; set; }
        public TypeOfScenario CurrentRoomType { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }

        public void CleanAllContexts()
        {
            Player = null;

            CurrentRoomContext?.ClearContext();
            CurrentRoomContext = null;
            CurrentRoomName = null;
            CurrentRoomType = default;
        }
    }
}