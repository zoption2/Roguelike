using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameStarter : MonoBehaviour
{
    private IPoolManager _poolManager;
    private IUIManager _UIManager;
    private Transform _globalPoolParent;

    [Inject]
    public void Construct(IPoolManager poolManager, IUIManager uIManager)
    {
        _poolManager = poolManager;
        _UIManager = uIManager;
    }

    public Transform GlobalPoolParent
    {
        get
        {
            if (_globalPoolParent == null)
            {
                GameObject globalParentObject = GameObject.Find("Pools");
                if (globalParentObject == null)
                {
                    globalParentObject = new GameObject("Pools");
                }
                _globalPoolParent = globalParentObject.transform;
            }
            return _globalPoolParent;
        }
    }

    void Start()
    {
        _poolManager.Init(GlobalPoolParent.gameObject);

        var container = FindObjectOfType<ProjectContext>().Container;
        container.Inject(this);

        LoadingScreenManager.SetScenesToLoad(new string[] { "Menu" });
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);

    }

}