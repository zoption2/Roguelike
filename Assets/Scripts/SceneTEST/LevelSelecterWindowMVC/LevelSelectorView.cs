using UnityEngine;
using UnityEngine.UI;

public interface ILevelSelectorView : IWindowView
{
    Button LoadLevelButton { get; }
    Button LoadMenuButton { get; }

    public GameObject GameObject { get; }
}


public class LevelSelectorView : MonoBehaviour, ILevelSelectorView
{
    [SerializeField]
    private Button _loadLevelBTN;

    [SerializeField]
    private Button _loadMenuBTN;

    public GameObject GameObject => gameObject;

    public Button LoadLevelButton => _loadLevelBTN;
    public Button LoadMenuButton => _loadMenuBTN;

    public void Init()
    {
    }
}
