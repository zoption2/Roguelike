using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IPoolManager
{
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
    public void Init(GameObject parent);
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
        switch (poolType)
        {
            case PoolType.BuffPool:
                InitSinglePool(_buffPooler);
                break;
            case PoolType.EffectPool:
                InitSinglePool(_effectPooler);
                break;
            case PoolType.AbilityIconPool:
                InitSinglePool(_abilityIconPooler);
                break;
            case PoolType.CharacterPanelPool:
                InitSinglePool(_characterPanelPooler);
                break;
            case PoolType.CharacterPool:
                InitSinglePool(_characterPooler);
                break;
            case PoolType.CharacterUIPool:
                InitSinglePool(_characterUIPooler);
                break;
            case PoolType.ProjectilePool:
                InitSinglePool(_projectilePooler);
                break;
            case PoolType.SlingshotPool:
                InitSinglePool(_slingshotPooler);
                break;
            default:
                Debug.LogWarning("Unknown pool type: " + poolType);
                break;
        }
    }

    private void InitSinglePool<TEnum>(IPool<TEnum> pool)
    {

        pool.Init(_globalParent);

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
