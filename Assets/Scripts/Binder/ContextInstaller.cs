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
    private PlayerPrefabHolder _playerPrefabHolder;

    [SerializeField]
    private SlingShotPrefabHolder _slingShotPrefabHolder;
    public override void InstallBindings()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsTransient();
        Container.Bind<ObjectPooler<PlayerType>>().To<PlayerPooler>().AsSingle();
        Container.Bind<ObjectPooler<SlingShotType>>().To<SlingshotPooler>().AsSingle();
        Container.Bind<PlayerPrefabHolder>().FromInstance(_playerPrefabHolder).AsSingle();
        Container.Bind<SlingShotPrefabHolder>().FromInstance(_slingShotPrefabHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();

        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
    }
}
