using UnityEngine;
using UnityEngine.UI;

public interface IMenuView : IWindowView
{
    Button SelectLevelButton { get; }
    Button StoreButton { get; }

    GameObject GameObject { get; }
}


public class MenuView : MonoBehaviour, IMenuView
{
    [SerializeField]
    private Button _selectLevelBTN;

    [SerializeField]
    private Button _storeBTN;

    public GameObject GameObject => gameObject;

    public Button SelectLevelButton => _selectLevelBTN;
    public Button StoreButton => _storeBTN;

    public void Init()
    {
    }
}