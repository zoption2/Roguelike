using Pool;
using UnityEngine;

public interface IPoolManager
{
    public BuffPooler UseBuffPooler();
    public EffectPooler UseEffectPooler();
    public AbilityIconPooler UseAbilityIconPooler();
    public CharacterPanelPooler UseCharacterPanelPooler();
    public CharacterPooler UseCharacterPooler();
    public CharacterUIPooler UseCharacterUIPooler();
    public ProjectilePooler UseProjectilePooler();
    public SlingshotPooler UseSlingshotPooler();
    public ParticlePooler UseParticlePooler();
    void InitPool(PoolType poolType, string poolName);
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
    private ParticlePooler _particlePooler;
    private Transform _globalParent;
    public PoolManager(
        BuffPooler buffPooler,
        EffectPooler effectPooler,
        AbilityIconPooler abilityIconPooler,
        CharacterPanelPooler characterPanelPooler,
        CharacterPooler characterPooler,
        CharacterUIPooler characterUIPooler,
        ProjectilePooler projectilePooler,
        SlingshotPooler slingshotPooler,
        ParticlePooler particlePooler
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
        _particlePooler = particlePooler;
    }

    public void Init(GameObject parent)
    {
        _globalParent = parent.transform;
    }

    public BuffPooler UseBuffPooler()
    {
        InitPool(PoolType.BuffPool, "BuffPool");
        return _buffPooler;
    }

    public EffectPooler UseEffectPooler()
    {
        InitPool(PoolType.EffectPool, "EffectPool");
        return _effectPooler;
    }

    public AbilityIconPooler UseAbilityIconPooler()
    {
        InitPool(PoolType.AbilityIconPool, "AbilityIconPool");
        return _abilityIconPooler;
    }

    public CharacterPanelPooler UseCharacterPanelPooler()
    {
        InitPool(PoolType.CharacterPanelPool, "CharacterPanelPool");
        return _characterPanelPooler;
    }

    public CharacterPooler UseCharacterPooler()
    {
        InitPool(PoolType.CharacterPool, "CharacterPool");
        return _characterPooler;
    }

    public CharacterUIPooler UseCharacterUIPooler()
    {
        InitPool(PoolType.CharacterUIPool, "CharacterUIPool");
        return _characterUIPooler;
    }

    public ProjectilePooler UseProjectilePooler()
    {
        InitPool(PoolType.ProjectilePool, "ProjectilePool");
        return _projectilePooler;
    }

    public SlingshotPooler UseSlingshotPooler()
    {
        InitPool(PoolType.SlingshotPool, "SlingshotPool");
        return _slingshotPooler;
    }

    public ParticlePooler UseParticlePooler()
    {
        InitPool(PoolType.ParticlePool, "ParticlePool");
        return _particlePooler;
    }

    public void InitPool(PoolType poolType, string poolName)
    {
        switch (poolType)
        {
            case PoolType.BuffPool:
                InitSinglePool(_buffPooler, poolName);
                break;
            case PoolType.EffectPool:
                InitSinglePool(_effectPooler, poolName);
                break;
            case PoolType.AbilityIconPool:
                InitSinglePool(_abilityIconPooler, poolName);
                break;
            case PoolType.CharacterPanelPool:
                InitSinglePool(_characterPanelPooler, poolName);
                break;
            case PoolType.CharacterPool:
                InitSinglePool(_characterPooler, poolName);
                break;
            case PoolType.CharacterUIPool:
                InitSinglePool(_characterUIPooler, poolName);
                break;
            case PoolType.ProjectilePool:
                InitSinglePool(_projectilePooler, poolName);
                break;
            case PoolType.SlingshotPool:
                InitSinglePool(_slingshotPooler, poolName);
                break;
            case PoolType.ParticlePool:
                InitSinglePool(_particlePooler, poolName);
                break;
            default:
                Debug.LogWarning("Unknown pool type: " + poolType);
                break;
        }
    }

    private void InitSinglePool<TEnum>(IPool<TEnum> pool, string poolName)
    {
        pool.Init(_globalParent, poolName);
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
        _particlePooler.CleanPool();


        foreach (Transform child in _globalParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
