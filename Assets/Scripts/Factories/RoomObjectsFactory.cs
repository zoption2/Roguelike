using Cysharp.Threading.Tasks;
using Prefab;
using UnityEngine;
using Zenject;

public interface IRoomObjectsFactory
{
    GameObject Build(Vector3 position, Transform parent, RoomObjectType type);
    public void Init();
}

public class RoomObjectsFactory : IRoomObjectsFactory
{
    [Inject]
    private RoomObjectsPrefabHolder _prefabHolder;

    public void Init()
    {
    }

    public GameObject Build(Vector3 position, Transform parent, RoomObjectType type)
    {
        GameObject prefab =  _prefabHolder.GetPrefab(type);

        return GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
    }
}
