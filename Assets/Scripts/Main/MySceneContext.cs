using UnityEngine;

namespace Gameplay
{
    public interface ISceneContextMarker
    {

    }
    public class MySceneContext : MonoBehaviour, ISceneContextMarker
    {
        [field: SerializeField] public Transform[] PlayersSpawnPoints { get; private set; }
        [field: SerializeField] public Transform[] EnemiesSpawnPoints { get; private set; }
    }
}


