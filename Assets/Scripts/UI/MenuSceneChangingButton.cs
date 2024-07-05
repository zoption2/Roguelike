using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MenuSceneChangingButton : MonoBehaviour
    {
        private const string _sceneName = "Loader";
        private ICharacterSelector _characterSelector;
        private ILevelManager _levelManager;


        [Inject]
        public void Construct(ICharacterSelector characterSelector, ILevelManager levelManager)
        {
            _characterSelector = characterSelector;
            _levelManager = levelManager;
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
                //SceneManager.LoadScene(_sceneName);
                _levelManager.LoadLevel();
            }
            else
            {
                Debug.LogWarning("Choose the characters first!");
            }
        }
    }
}
