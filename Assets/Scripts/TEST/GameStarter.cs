using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameStarter : MonoBehaviour
{
    [Inject]
    private IPoolManager _poolManager;
    private Transform _globalPoolParent;

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
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

}
