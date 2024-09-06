using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BackToMenuButton : MonoBehaviour
{
    private Button _button;

    private IUIManager _UIManager;

    [Inject]
    public void Construct(IUIManager uiManager)
    {
        _UIManager = uiManager;
    }

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(DoSOmeButtonLogic);

        var container = FindObjectOfType<ProjectContext>().Container;
        container.Inject(this);
    }

    private void DoSOmeButtonLogic()
    {
        _UIManager.ShowUIElement(UIElementType.Menu);
    }
}
