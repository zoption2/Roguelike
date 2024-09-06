using UnityEngine;
using Zenject;

public interface IGameManager
{
    public void Init();
}

public class GameManager : IGameManager
{
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

    [Inject]
    public void Construct(IPoolManager poolManager)
    {
        _poolManager = poolManager;
    }

    public void Init()
    {
        _poolManager.Init(GlobalPoolParent.gameObject);
    }

}
