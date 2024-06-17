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

        public void Start()
        {
            _gameplayService.InitRoom();
        }
    }
}

