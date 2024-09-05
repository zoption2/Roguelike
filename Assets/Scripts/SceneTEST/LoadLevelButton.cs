using UnityEngine;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenSelectLevelWindow);
    }

    private void OpenSelectLevelWindow()
    {
        Debug.LogWarning("OpenStoreWindow");
    }
}
