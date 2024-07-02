using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPoolManager
{
    void Init(GameObject parent);
    BuffPooler UseBuffPooler();
    EffectPooler UseEffectPooler();
    AbilityIconPooler UseAbilityIconPooler();
    CharacterPanelPooler UseCharacterPanelPooler();
    CharacterPooler UseCharacterPooler();
    CharacterUIPooler UseCharacterUIPooler();
    ProjectilePooler UseProjectilePooler();
    SlingshotPooler UseSlingshotPooler();
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
    private Transform _globalParent;

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

    public void Init(GameObject parent)
    {
        _globalParent = parent.transform;
    }

    public BuffPooler UseBuffPooler()
    {
        InitPool(PoolType.BuffPool);
        return _buffPooler;
    }

    public EffectPooler UseEffectPooler()
    {
        InitPool(PoolType.EffectPool);
        return _effectPooler;
    }

    public AbilityIconPooler UseAbilityIconPooler()
    {
        InitPool(PoolType.AbilityIconPool);
        return _abilityIconPooler;
    }

    public CharacterPanelPooler UseCharacterPanelPooler()
    {
        InitPool(PoolType.CharacterPanelPool);
        return _characterPanelPooler;
    }

    public CharacterPooler UseCharacterPooler()
    {
        InitPool(PoolType.CharacterPool);
        return _characterPooler;
    }

    public CharacterUIPooler UseCharacterUIPooler()
    {
        InitPool(PoolType.CharacterUIPool);
        return _characterUIPooler;
    }

    public ProjectilePooler UseProjectilePooler()
    {
        InitPool(PoolType.ProjectilePool);
        return _projectilePooler;
    }

    public SlingshotPooler UseSlingshotPooler()
    {
        InitPool(PoolType.SlingshotPool);
        return _slingshotPooler;
    }

    public void InitPool(PoolType poolType)
    {
        if (_globalParent == null)
        {
            Debug.LogError("Parent for pools is`t set before pool.Init.");
            return;
        }

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

    private Transform CreateParentTransform(string poolName)
    {
        GameObject parentObject = new GameObject(poolName);
        parentObject.transform.SetParent(_globalParent);
        Transform parentTransform = parentObject.transform;
        parentTransform.localPosition = Vector3.zero;
        parentTransform.localRotation = Quaternion.identity;
        return parentTransform;
    }

    private void InitSinglePool<TEnum>(IPool<TEnum> pool, Transform parent)
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
