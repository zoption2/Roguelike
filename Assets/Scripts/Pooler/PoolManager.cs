using Pool;
using UnityEngine;
using Zenject;

public interface IPoolManager
{
    BuffPooler GetBuffPooler();
    EffectPooler GetEffectPooler();
    AbilityIconPooler GetAbilityIconPooler();
    CharacterPanelPooler GetCharacterPanelPooler();
    CharacterPooler GetCharacterPooler();
    CharacterUIPooler GetCharacterUIPooler();
    ProjectilePooler GetProjectilePooler();
    SlingshotPooler GetSlingshotPooler();

    void InitPoolers(Transform parent);
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

    public void InitPoolers(Transform parent)
    {
        CreatePool(_buffPooler, "BuffPool", parent);
        CreatePool(_effectPooler, "EffectPool", parent);
        CreatePool(_abilityIconPooler, "AbilityIconPool", parent);
        CreatePool(_characterPanelPooler, "CharacterPanelPool", parent);
        CreatePool(_characterPooler, "CharacterPool", parent);
        CreatePool(_characterUIPooler, "CharacterUIPool", parent);
        CreatePool(_projectilePooler, "ProjectilePool", parent);
        CreatePool(_slingshotPooler, "SlingshotPool", parent);

        //_buffPooler.Init(buffParent);
        //_effectPooler.Init(effectParent);
        //_abilityIconPooler.Init(abilityIconParent);
        //_characterPanelPooler.Init(characterPanelParent);
        //_characterPooler.Init(characterParent);
        //_characterUIPooler.Init(characterUIParent);
        //_projectilePooler.Init(projectileParent);
        //_slingshotPooler.Init(slingshotParent);
    }

    private void CreatePool<T>(IPool<T> pool, string name, Transform parent)
    {
        GameObject poolParent = new GameObject(name);
        poolParent.transform.SetParent(parent);
        pool.Init();
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

    public BuffPooler GetBuffPooler() { return _buffPooler; }
    public EffectPooler GetEffectPooler() { return _effectPooler; }
    public AbilityIconPooler GetAbilityIconPooler() { return _abilityIconPooler; }
    public CharacterPanelPooler GetCharacterPanelPooler() { return _characterPanelPooler; }
    public CharacterPooler GetCharacterPooler() { return _characterPooler; }
    public CharacterUIPooler GetCharacterUIPooler() { return _characterUIPooler; }
    public ProjectilePooler GetProjectilePooler() { return _projectilePooler; }
    public SlingshotPooler GetSlingshotPooler() { return _slingshotPooler; }
}
