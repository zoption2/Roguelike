using UI;
using UnityEngine;
using Zenject;

public class MenuController : IUIWindowController
{
    private IMenuView _windowView;
    UIWindowModel _windowModel;

    [Inject]
    private IUIManager _UIManager;

    public void Init(IWindowView view, UIWindowModel model)
    {
        _windowView = view as IMenuView;
        _windowModel = model;

        _windowView.SelectLevelButton.onClick.AddListener(LoadSelectLevelWindow);
        _windowView.StoreButton.onClick.AddListener(LoadStore);
    }

    public void LoadSelectLevelWindow()
    {
        _UIManager.ShowUIElement(UIElementType.LevelSelector);
    }

    public void LoadStore()
    {
        Debug.Log("Store is close now :(");
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
