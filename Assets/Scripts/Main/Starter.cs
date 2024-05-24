using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Starter : MonoBehaviour
    {
        private IGameplayService _gameplayService;

        [SerializeField]
        private DefaultScenarioContext _sceneContext;

        [Inject]
        public void Construct(IGameplayService service)
        {
            _gameplayService = service;
        }

        public void Start()
        {
            // Load Level scenario
            //Level scenario will load a room scenario (roomSetings)
            _gameplayService.Init(TypeOfScenario.Default, _sceneContext);
        }
    }
}

