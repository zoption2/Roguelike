using Prefab;
using UnityEngine;
using Zenject;

public interface IChestFactory
{
    public IChestController CreateChest(Vector3 position, Transform parent);
}
public class ChestFactory : IChestFactory
{
    private RoomObjectsPrefabHolder _holder;
    private DiContainer _container;

    public ChestFactory(RoomObjectsPrefabHolder holder, DiContainer container)
    {
        _holder = holder;
        _container = container;
    }

    public IChestController CreateChest(Vector3 position, Transform parent)
    {
        IChestController controller = GetNewController();

        ChestModel model = new ChestModel();
        GameObject prefab = _holder.GetPrefab(TemplateElementType.Chest);
        GameObject chest = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        IChestView chestView = chest.GetComponent<IChestView>();

        controller.Init(chestView, model);

        return controller;
    }

    protected IChestController GetNewController()
    {
        return _container.Resolve<IChestController>();
    }
}
