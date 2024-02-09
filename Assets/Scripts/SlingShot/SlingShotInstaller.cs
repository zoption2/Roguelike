using SlingShot;
using System.ComponentModel;
using UnityEngine;
using Zenject;

public class SlingShotInstaller : MonoInstaller
{
    //[SerializeField] slingShotSO _slingShotControllerPrefab;
    public override void InstallBindings()
    {
        //Container.Bind<ISlingShot>().To<SlingShotController>().AsSingle();
        //Container.Bind<slingShotSO>().FromInstance(_slingShotControllerPrefab).AsSingle();
        //Container.Bind<IPlayer>().To<Player>().AsSingle();
    }
}
