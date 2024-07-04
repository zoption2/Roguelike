using Prefab;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Cysharp.Threading.Tasks;

public interface INavigationFactory
{
    public UniTask<NavMeshSurface> CreateNavigation();
}
public class NavigationFactory : INavigationFactory
{
    private NavigationPrefabHolder _navigationPrefabHolder;
    public NavigationFactory(NavigationPrefabHolder navigationPrefabHolder)
    {
        _navigationPrefabHolder = navigationPrefabHolder;
    }
    
    public async UniTask<NavMeshSurface> CreateNavigation()
    {
        GameObject prefab =  await _navigationPrefabHolder.GetPrefabAsync(NavigationType.Default);
        GameObject navObj = GameObject.Instantiate(prefab, Vector3.zero, prefab.transform.rotation);
        NavMeshSurface navigation = navObj.GetComponent<NavMeshSurface>();
        return navigation;
    }
}
