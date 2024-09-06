using UnityEngine;
using Zenject;

public interface IUIWindowFactory
{
    public IUIWindowController CreateWindow(UIElementType type, Transform transform);
}


public class UIWindowFactory : IUIWindowFactory
{
    private UIPrefabHolder _UIprefabHolder;
    private DiContainer _container;

    [Inject]
    public void Construct(
        UIPrefabHolder uIPrefabHolder,
        DiContainer diContainer)
    {
        _UIprefabHolder = uIPrefabHolder;
        _container = diContainer;
    }

    public IUIWindowController CreateWindow(UIElementType type, Transform transform)
    {
        UIWindowModel model = new UIWindowModel();

        GameObject prefab = _UIprefabHolder.GetPrefab(type);
        GameObject instance = GameObject.Instantiate(prefab);
        IWindowView view = instance.GetComponent<IWindowView>();
        instance.SetActive(false);

        IUIWindowController controller = GetWindowController(type);
        controller.Init(view, model);

        instance.transform.SetParent(transform, false);

        return controller;
    }

    public IUIWindowController GetWindowController(UIElementType type)
    {
        switch (type)
        {
            case UIElementType.Menu:
                return _container.ResolveId<IUIWindowController>(type.ToString());

            case UIElementType.LevelSelector:
                return _container.ResolveId<IUIWindowController>(type.ToString());

            default:
                return null;
        }
    }
}
