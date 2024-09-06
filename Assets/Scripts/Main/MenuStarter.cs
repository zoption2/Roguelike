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
    private IUIWindowFactory _UIWindowFactory;
    //private ICharacterSelector _characterSelector;
    //private RectTransform _rectTransform;
    //[SerializeField]
    //private int _requiredPlayers = 1;

    [Inject]
    public void Construct(
        ICharacterSelector characterSelector,
        IUIManager uIManager,
        IPoolManager poolManager,
        IUIWindowFactory uIWindowFactory)
    {
        _UIManager = uIManager;
        _UIPooler = poolManager.UseUIPooler();
        _UIWindowFactory = uIWindowFactory;
    }

    void Start()
    {
        _UIManager.CreateUIElement(UIElementType.Menu);
        _UIManager.CreateUIElement(UIElementType.LevelSelector);

        _UIManager.ShowUIElement(UIElementType.Menu);
    }

}
