using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour
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
