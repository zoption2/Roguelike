using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class RoomStarter : MonoBehaviour
    {
        private IGameplayService _gameplayService;

        public void Init(IGameplayService service)
        {
            _gameplayService = service;
        }

        public void StartRoom(TypeOfScenario type)
        {
            _gameplayService.Init(type);
        }
    }
}

