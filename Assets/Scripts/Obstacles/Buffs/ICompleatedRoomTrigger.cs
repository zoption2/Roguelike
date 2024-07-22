
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
        public void SetExitType(TemplateElementType exitType);
        public Transform Transform { get; set; }
        public TemplateElementType GetExitType();

        public void SetExitDirection(ExitDirection exitDirection);
        public ExitDirection GetExitDirection();
        public void Init(IGameplayService gameplayService, ILevelManager levelManager);
    }
}