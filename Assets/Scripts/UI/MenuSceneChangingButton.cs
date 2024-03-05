using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MenuSceneChangingButton : MonoBehaviour
    {
        private const string _sceneName = "PlayerTestScene";

        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(SwitchScene);
        }

        private void SwitchScene()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}
