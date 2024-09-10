using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public interface ILevelSelectorView : IWindowView
{
    Button LoadLevelButton { get; }
    Button LoadMenuButton { get; }
    public void Init(LevelSelectorController levelSelectorController);
    public GameObject GameObject { get; }
    public List<ICharacterPanelView> GetCharacterPanels();
    public List<GameObject> CharacterPanels { get; }
    public List<GameObject> LevelPanels { get; }
    public Transform CharacterPanelsContainer { get; }
    public MagneticScroll LevelScroll { get;  }
    public MagneticScroll CharacterScroll { get; }

    public MagneticScroll GetLevelMagneticScroll();
    public MagneticScroll GetCharacterMagneticScroll();
}


public class LevelSelectorView : MonoBehaviour, ILevelSelectorView
{
    [SerializeField] private Button _loadLevelBTN;
    [SerializeField] private Button _loadMenuBTN;

    [SerializeField] private MagneticScroll _levelScroll;
    [SerializeField] private MagneticScroll _characterScroll;

    [SerializeField] private Transform _characterContainer;
    [SerializeField] private Transform _levelContainer;

    private List<ICharacterPanelView> _characterPanels;

    private LevelSelectorController _controller;

    public List<GameObject> CharacterPanels { get; private set; } = new List<GameObject>();
    public List<GameObject> LevelPanels { get; private set; } = new List<GameObject>();
    public Transform CharacterPanelsContainer  => _characterContainer;
    public MagneticScroll LevelScroll => _levelScroll;
    public MagneticScroll CharacterScroll => _characterScroll;

    public GameObject GameObject => gameObject;
    public Button LoadLevelButton => _loadLevelBTN;
    public Button LoadMenuButton => _loadMenuBTN;

    public void Init(LevelSelectorController levelSelectorController)
    {
        _controller = levelSelectorController;
        _characterPanels = new List<ICharacterPanelView>();

        foreach (Transform characterPanel in _characterContainer)
        {
            var panelView = characterPanel.GetComponent<ICharacterPanelView>();
            if (panelView != null)
            {
                _characterPanels.Add(panelView);
            }
        }

        var characterMS = GetCharacterMagneticScroll();
        characterMS.Init(this);

        foreach (Transform levelPanel in _levelContainer)
        {
            LevelPanels.Add(levelPanel.gameObject);
        }
    }


    public MagneticScroll GetLevelMagneticScroll()
    {
        return _levelScroll.GetComponent<MagneticScroll>();
    }

    public MagneticScroll GetCharacterMagneticScroll()
    {
        return _characterScroll.GetComponent<MagneticScroll>();
    }

    public List<ICharacterPanelView> GetCharacterPanels()
    {
        Debug.Log("Character panels count: " + _characterPanels.Count);
        return _characterPanels;
    }

}
