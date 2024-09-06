using UI;
using UnityEngine;
using Zenject;

public class LevelSelectorController : IUIWindowController
{
    private ILevelSelectorView _windowView;
    UIWindowModel _windowModel;

    [Inject]
    private IUIManager _UIManager;

    public void Init(IWindowView view, UIWindowModel model)
    {
        _windowView = view as ILevelSelectorView;
        _windowModel = model;

        _windowView.LoadLevelButton.onClick.AddListener(LoadLevel);
        _windowView.LoadMenuButton.onClick.AddListener(LoadMenu);
    }

    public void LoadLevel()
    {
        Debug.Log("Game will be starting sooooon");
    }

    public void LoadMenu()
    {
        _UIManager.ShowUIElement(UIElementType.Menu);
    }

    public void SetActive()
    {
        _windowView.GameObject.SetActive(true);
    }

    public void SetDisactive()
    {
        _windowView.GameObject.SetActive(false);
    }


}
