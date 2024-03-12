using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MenuSceneChangingButton : MonoBehaviour
    {
        private const string _sceneName = "PlayerTestScene";
        private ICharacterSelector _characterSelector;


        [Inject]
        public void Construct(ICharacterSelector characterSelector)
        {
            _characterSelector = characterSelector;
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
                SceneManager.LoadScene(_sceneName);
            }
            else
                Debug.LogWarning("Choose the characters first!");
        }
    }
}
