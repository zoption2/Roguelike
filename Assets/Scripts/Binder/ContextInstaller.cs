using Gameplay;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ContextInstaller : MonoInstaller
{
    [SerializeField]
    private PrefabHolder _prefabHolder;
    public override void InstallBindings()
    {
        Container.Bind<ICharacterController>().To<PlayerController>().AsSingle();
        Container.Bind<PrefabHolder>().FromInstance(_prefabHolder).AsSingle();
        Container.Bind<IPooler>().To<ObjectPooler>().AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
    }
}
