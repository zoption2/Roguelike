//using Gameplay;
//using Player;
//using Enemy;
//using CharactersStats;
//using Pool;
//using Prefab;
//using UnityEngine;
//using Zenject;

//public interface ICharacterFactory<T>
//{
//    public T CreatePlayer(Transform point, CharacterType type, ICharacterScenarioContext characters, int id = 0);
//}
//public class CharacterFactory<T> : ICharacterFactory<T> 
//{
//    private IStatsProvider _statsProvider;
//    private DiContainer _container;
//    private ObjectPooler<CharacterType> _pooler;

//    [Inject]
//    public void Construct(
//        DiContainer container,
//        IStatsProvider statsProvider,
//        ObjectPooler<CharacterType> pooler)
//    {
//        _container = container;
//        _statsProvider = statsProvider;
//        _pooler = pooler;
//        _pooler.Init();
//    }

//    public T CreateCharacter(Transform point, CharacterType type, ICharacterScenarioContext characters, int id = 0)
//    {
//        IPlayerView playerView;
//        ICharacterController controller;
//        Transform poolableTransform;
//        Rigidbody2D playerViewRigidbody;
//        PlayerModel playerModel;
//        Stats stats;

//        controller = GetNewController();

//        stats = _statsProvider.GetPlayerStats(type, id);

//        playerModel = new PlayerModel(id, type, stats);

//        IMyPoolable _poolable = _pooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
//        playerView = _poolable.gameObject.GetComponent<PlayerView>();

//        GameObject _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
//        poolableTransform = _poolableObj.transform;

//        playerViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

//        playerView.Initialize((PlayerController)controller);

//        controller.Init(poolableTransform, playerViewRigidbody, playerModel);

//        Debug.Log($"Player {type} with id {id} was created. They have {stats.Health} hp and spawned on {point.position}");

//        return controller;
//    }

//    private T GetNewController()
//    {
//        return _container.Resolve<T>();
//    }
//}