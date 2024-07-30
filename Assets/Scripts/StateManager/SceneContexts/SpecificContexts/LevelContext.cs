using Cinemachine;
using Player;
using Unity.AI.Navigation;

namespace Gameplay
{
    public interface ILevelContext : IScenarioContext
    {
        IPlayerController Player { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }
        public CinemachineVirtualCamera VirtualCamera { get; set; }
        public void CleanAllContexts();
    }

    public class LevelContext : ILevelContext
    {
        public IPlayerController Player { get; set; }
        public NavMeshSurface NavMeshSurface { get; set; }
        public CinemachineVirtualCamera VirtualCamera { get; set; }

        public void CleanAllContexts()
        {
            Player = null;
        }
    }
}