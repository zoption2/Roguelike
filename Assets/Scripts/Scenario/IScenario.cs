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
        public IGameplayService _gameplayService { get; set; }
        public void LoadMainMenu();
        public void CreateNewRoomScene(int level, int room);
    }
}
