using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Starter : MonoBehaviour
    {
        IGameplayService _gameplayService;

        [SerializeField] private DefaultScenarioContext _sceneContext;

        [Inject]
        public void Construct(IGameplayService service)
        {
            _gameplayService = service;
  
        }

        public void Start()
        {

            _gameplayService.Init(TypeOfScenario.Default, _sceneContext);
        }
    }
}

