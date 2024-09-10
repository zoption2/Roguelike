using UI;
using UnityEngine;
using Zenject;

public class LevelSelectorController : IUIWindowController
{
    private ILevelSelectorView _windowView;
    UIWindowModel _windowModel;

    private IUIManager _UIManager;
    private UIPooler _UIPooler;
    private ICharacterSelector _characterSelector;

    [Inject]
    public void Construct(IUIManager uIManager, IPoolManager poolManager, ICharacterSelector characterSelector)
    {
        _UIManager = uIManager;
        _characterSelector = characterSelector;
        _UIPooler = poolManager.UseUIPooler();
    }

    public void Init(IWindowView view, UIWindowModel model)
    {
        _windowView = view as ILevelSelectorView;
        _windowModel = model;

        _windowView.Init(this);
        _characterSelector.Init(1, _windowView.CharacterPanelsContainer);

        _windowView.LoadLevelButton.onClick.AddListener(LoadLevel);
        _windowView.LoadMenuButton.onClick.AddListener(LoadMenu);
    }

    public void AddCharacterPanel(CharacterType characterType)
    {
        _characterSelector.AddPanel(characterType);
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
