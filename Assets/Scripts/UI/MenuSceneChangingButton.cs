using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MenuSceneChangingButton : MonoBehaviour
    {
        private const string _sceneName = "Level_1";
        //Level_TestScene Level_1 Level_MovementTest Level_UITestScene Level_AdaptiveScreenTest
        private ICharacterSelector _characterSelector;
        private IGameplayService _gameplayService;

        [Inject]
        public void Construct(ICharacterSelector characterSelector, IGameplayService gameplayService)
        {
            _characterSelector = characterSelector;
            _gameplayService = gameplayService;
        }

        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(SwitchScene);
        }

        private void SwitchScene()
        {
            if (_characterSelector.HasRequiredNumberOfPlayers())
            {
                //var context = ScriptableObject.CreateInstance<DefaultScenarioContext>();
                //_gameplayService.EnqueueScenario(context);
                SceneManager.LoadScene(_sceneName);
                
            }
            else
            {
                Debug.LogWarning("Choose the characters first!");
            }
        }
    }
}
