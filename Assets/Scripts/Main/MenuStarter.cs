using UI;
using UnityEngine;
using Zenject;

public class MenuStarter : MonoBehaviour
{
    private UIPooler _UIPooler;
    //private IUIManager _UIManager;
    private IUIManager _UIManager;
    [SerializeField]
    private GameObject _levelSelectorWindow;
    //private ICharacterSelector _characterSelector;
    //private RectTransform _rectTransform;
    [SerializeField]
    private int _requiredPlayers = 1;

    [Inject]
    public void Construct(
        ICharacterSelector characterSelector,
        IUIManager uIManager,
        IPoolManager poolManager)
    {
        //_characterSelector = characterSelector;
        _UIManager = uIManager;
        _UIPooler = poolManager.UseUIPooler();
    }

    void Start()
    {
        //_levelSelectorWindow.SetActive(false);

        _UIManager.CreateUIElement(UIElementType.Menu);
        _UIManager.CreateUIElement(UIElementType.LevelSelector);

        _UIManager.ShowUIElement(UIElementType.Menu);

        //_UIManager.ShowUIElement(UIElementType.Menu);
        //UIElement menu = _UIPooler.Pull<UIElement>(UIElementType.Menu, Vector3.zero, Quaternion.identity, transform);
        //menu.transform.SetParent(transform.parent, false);

        //ProjectContext.Instance.Container.Inject(menu);

        //_rectTransform = menu.GetComponent<RectTransform>();
        //_characterSelector.Init(_requiredPlayers, _rectTransform);
    }

}
