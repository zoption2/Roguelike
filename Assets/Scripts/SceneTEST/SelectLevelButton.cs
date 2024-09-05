using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SelectLevelButton : MonoBehaviour
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
        _button.onClick.AddListener(OpenSelectLevelWindow);

        var container = FindObjectOfType<ProjectContext>().Container;
        container.Inject(this);
    }

    private void OpenSelectLevelWindow()
    {
        _UIManager.ShowUIElement(UIElementType.LevelSelector);
    }
}
