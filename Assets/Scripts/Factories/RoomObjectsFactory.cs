using Cysharp.Threading.Tasks;
using Prefab;
using UnityEngine;
using Zenject;

public interface IRoomObjectsFactory
{
    UniTask<GameObject> Build(Vector3 position, Transform parent, RoomObjectType type);
    public void Init();
}

public class RoomObjectsFactory : IRoomObjectsFactory
{
    [Inject]
    private RoomObjectsPrefabHolder _prefabHolder;

    public void Init()
    {
    }

    public async UniTask<GameObject> Build(Vector3 position, Transform parent, RoomObjectType type)
    {
        GameObject prefab = await _prefabHolder.GetPrefab(type);

        return GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
    }
}
