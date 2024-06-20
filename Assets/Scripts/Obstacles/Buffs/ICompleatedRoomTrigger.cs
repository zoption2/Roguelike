
using Gameplay;
using UnityEngine;

namespace Obstacles
{
    public interface ICompleatedRoomTrigger
    {
        public void ActivateTrigger();
        public void UseTrigger(GameObject player);
        public void DisableTrigger();
        public void Init(IGameplayService gameplayService);
        public bool GetActiveStatus();
    }
}