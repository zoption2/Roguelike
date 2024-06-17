
using Gameplay;

namespace Obstacles
{
    public interface ICompleatedRoomTrigger
    {
        public void ActivateTrigger();
        public void UseTrigger();
        public void DisableTrigger();
        public void Init(IScenario scenario);
        public bool GetActiveStatus();
    }
}