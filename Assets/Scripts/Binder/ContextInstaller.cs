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
    private PlayerPrefabHolder _prefabHolder;
    public override void InstallBindings()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
        Container.Bind<ObjectPooler<PlayerType>>().To<PlayerPooler>().AsSingle();
        Container.Bind<PlayerPrefabHolder>().FromInstance(_prefabHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
    }
}
