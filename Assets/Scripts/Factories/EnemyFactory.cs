using Enemy;
using Gameplay;
using Player;
using Pool;
using Prefab;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, EnemyType type, int id = 0);
}
public class EnemyFactory : IEnemyFactory
{
    private IEnemyController _controller;
    private IGameplayService _gameplayService;
    private IEnemyView _enemyView;
    private ObjectPooler<EnemyType> _enemyPooler;

    [Inject]
    public void Construct(
        IEnemyController controller,
        IGameplayService service,
        ObjectPooler<EnemyType> enemyPooler
        )
    {
        //_controller = controller;
        _gameplayService = service;
        _enemyPooler = enemyPooler;
    }

    public IEnemyController CreateEnemy(Transform point, EnemyType type, int id = 0)
    {
        Transform _poolableTransform;
        Rigidbody2D _enemyViewRigidbody;
        EnemyModel _enemyModel;
        Stats _stats;
        _controller = new EnemyController();    
        _enemyPooler.Init();

        _stats = _gameplayService._statsProvider.GetEnemyStats(type, id);

        _enemyModel = new EnemyModel(id, type, _stats);

        var _poolable = _enemyPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        _enemyView = _poolable.gameObject.GetComponent<MyEnemyView>();

        var _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
        _poolableTransform = _poolableObj.transform;

        _enemyViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        _enemyView.Initialize((EnemyController)_controller);

        _controller.Init(_poolableTransform, _enemyViewRigidbody, _enemyModel);

        _gameplayService.Enemies.Add(_controller);

        Debug.Log($"Enemy of type {type} was created. They have {_stats.Health} hp and spawned on {point.position}");

        return _controller;
    }
}


