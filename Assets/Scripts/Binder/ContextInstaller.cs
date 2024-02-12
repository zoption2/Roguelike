using Gameplay;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Pool;
using Prefab;
public class ContextInstaller : MonoInstaller
{
    [SerializeField]
    private PrefabHolder _prefabHolder;
    public override void InstallBindings()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
        Container.Bind<IPrefabByEnumProvider>().To<PrefabHolder>().AsSingle();
        Container.Bind<IPooler>().To<ObjectPooler>().AsSingle();
        Container.Bind<PrefabHolder>().FromInstance(_prefabHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
    }
}
