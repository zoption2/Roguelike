
using Gameplay;
using UnityEngine;

namespace Obstacles
{
    public interface ICompleatedRoomTrigger
    {
        public void ActivateTrigger();
        public void UseTrigger();
        public void DisableTrigger();
        public bool GetActiveStatus();
    }
}