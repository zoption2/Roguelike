using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public interface IScenario
    {
        object GetScenarioContext();
        public void OnStateEnd();
        public void Init(IScenarioContext context, LevelManager levelManager);
        public IGameplayService GameplayService { get; set; }
        public void LoadMainMenu();
        public void RenewQueue();
    }
}
