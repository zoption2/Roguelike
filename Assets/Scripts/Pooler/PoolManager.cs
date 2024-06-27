using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPoolManager
{
    IPool<TEnum> UsePooler<TEnum>(PoolType poolType);
    void InitPool(PoolType poolType);
    void CleanPoolers();
}

public class PoolManager : IPoolManager
{
    private BuffPooler _buffPooler;
    private EffectPooler _effectPooler;
    private AbilityIconPooler _abilityIconPooler;
    private CharacterPanelPooler _characterPanelPooler;
    private CharacterPooler _characterPooler;
    private CharacterUIPooler _characterUIPooler;
    private ProjectilePooler _projectilePooler;
    private SlingshotPooler _slingshotPooler;

    private Dictionary<PoolType, Transform> _parentTransforms;

    [Inject]
    public void Construct(
        BuffPooler buffPooler,
        EffectPooler effectPooler,
        AbilityIconPooler abilityIconPooler,
        CharacterPanelPooler characterPanelPooler,
        CharacterPooler characterPooler,
        CharacterUIPooler characterUIPooler,
        ProjectilePooler projectilePooler,
        SlingshotPooler slingshotPooler
    )
    {
        _buffPooler = buffPooler;
        _effectPooler = effectPooler;
        _abilityIconPooler = abilityIconPooler;
        _characterPanelPooler = characterPanelPooler;
        _characterPooler = characterPooler;
        _characterUIPooler = characterUIPooler;
        _projectilePooler = projectilePooler;
        _slingshotPooler = slingshotPooler;

        _parentTransforms = new Dictionary<PoolType, Transform>();
    }

    public void InitPool(PoolType poolType)
    {
        if (!_parentTransforms.ContainsKey(poolType))
        {
            Transform parent = CreateParentTransform(poolType.ToString());
            _parentTransforms[poolType] = parent;
        }

        Transform parentTransform = _parentTransforms[poolType];

        switch (poolType)
        {
            case PoolType.BuffPool:
                InitSinglePool(_buffPooler, parentTransform);
                break;
            case PoolType.EffectPool:
                InitSinglePool(_effectPooler, parentTransform);
                break;
            case PoolType.AbilityIconPool:
                InitSinglePool(_abilityIconPooler, parentTransform);
                break;
            case PoolType.CharacterPanelPool:
                InitSinglePool(_characterPanelPooler, parentTransform);
                break;
            case PoolType.CharacterPool:
                InitSinglePool(_characterPooler, parentTransform);
                break;
            case PoolType.CharacterUIPool:
                InitSinglePool(_characterUIPooler, parentTransform);
                break;
            case PoolType.ProjectilePool:
                InitSinglePool(_projectilePooler, parentTransform);
                break;
            case PoolType.SlingshotPool:
                InitSinglePool(_slingshotPooler, parentTransform);
                break;
            default:
                Debug.LogWarning("Unknown pool type: " + poolType);
                break;
        }
    }

    public IPool<TEnum> UsePooler<TEnum>(PoolType poolType)
    {
        InitPool(poolType);

        switch (poolType)
        {
            case PoolType.BuffPool:
                return _buffPooler as IPool<TEnum>;
            case PoolType.EffectPool:
                return _effectPooler as IPool<TEnum>;
            case PoolType.AbilityIconPool:
                return _abilityIconPooler as IPool<TEnum>;
            case PoolType.CharacterPanelPool:
                return _characterPanelPooler as IPool<TEnum>;
            case PoolType.CharacterPool:
                return _characterPooler as IPool<TEnum>;
            case PoolType.CharacterUIPool:
                return _characterUIPooler as IPool<TEnum>;
            case PoolType.ProjectilePool:
                return _projectilePooler as IPool<TEnum>;
            case PoolType.SlingshotPool:
                return _slingshotPooler as IPool<TEnum>;
            default:
                Debug.LogWarning("Unknown pool type: " + poolType);
                return null;
        }
    }

    private Transform CreateParentTransform(string poolName)
    {
        GameObject parentObject = GameObject.Find(poolName);
        if (parentObject == null)
        {
            parentObject = new GameObject(poolName);
        }
        Transform parentTransform = parentObject.transform;
        parentTransform.localPosition = Vector3.zero;
        parentTransform.localRotation = Quaternion.identity;
        return parentTransform;
    }

    private void InitSinglePool<T>(IPool<T> pool, Transform parent)
    {
        pool.Init(parent);
    }

    public void CleanPoolers()
    {
        _buffPooler.CleanPool();
        _effectPooler.CleanPool();
        _abilityIconPooler.CleanPool();
        _characterPanelPooler.CleanPool();
        _characterPooler.CleanPool();
        _characterUIPooler.CleanPool();
        _projectilePooler.CleanPool();
        _slingshotPooler.CleanPool();
    }
}
